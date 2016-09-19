using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using InvoiceGenerator.Data;
using InvoiceGenerator.Entities;
using Microsoft.AspNet.Cryptography.KeyDerivation;
using Newtonsoft.Json;

namespace InvoiceGenerator.API.Controllers
{
  public class Payload
  {
    public string iss { get; set; }
    public string sub { get; set; }
  }

  public interface IHashHelper
  {
    string HashPassword(string password);
    bool VerifyHashedPassword(string hashedPassword, string providedPassword);
    string Encode<T>(T payload, string secret);
    string Decode(string[] segments, string secret);
  }

  public class HashHelper : IHashHelper
  {
    public string HashPassword(string password)
    {
      var prf = KeyDerivationPrf.HMACSHA256;
      var rng = RandomNumberGenerator.Create();
      const int iterCount = 10000;
      const int saltSize = 128 / 8;
      const int numBytesRequested = 256 / 8;

      // Produce a version 3 (see comment above) text hash.
      var salt = new byte[saltSize];
      rng.GetBytes(salt);
      var subkey = KeyDerivation.Pbkdf2(password, salt, prf, iterCount, numBytesRequested);

      var outputBytes = new byte[13 + salt.Length + subkey.Length];
      outputBytes[0] = 0x01; // format marker
      WriteNetworkByteOrder(outputBytes, 1, (uint)prf);
      WriteNetworkByteOrder(outputBytes, 5, iterCount);
      WriteNetworkByteOrder(outputBytes, 9, saltSize);
      Buffer.BlockCopy(salt, 0, outputBytes, 13, salt.Length);
      Buffer.BlockCopy(subkey, 0, outputBytes, 13 + saltSize, subkey.Length);
      return Convert.ToBase64String(outputBytes);
    }

    public bool VerifyHashedPassword(string hashedPassword, string providedPassword)
    {
      var decodedHashedPassword = Convert.FromBase64String(hashedPassword);

      // Wrong version
      if (decodedHashedPassword[0] != 0x01)
        return false;

      // Read header information
      var prf = (KeyDerivationPrf)ReadNetworkByteOrder(decodedHashedPassword, 1);
      var iterCount = (int)ReadNetworkByteOrder(decodedHashedPassword, 5);
      var saltLength = (int)ReadNetworkByteOrder(decodedHashedPassword, 9);

      // Read the salt: must be >= 128 bits
      if (saltLength < 128 / 8)
      {
        return false;
      }
      var salt = new byte[saltLength];
      Buffer.BlockCopy(decodedHashedPassword, 13, salt, 0, salt.Length);

      // Read the subkey (the rest of the payload): must be >= 128 bits
      var subkeyLength = decodedHashedPassword.Length - 13 - salt.Length;
      if (subkeyLength < 128 / 8)
      {
        return false;
      }
      var expectedSubkey = new byte[subkeyLength];
      Buffer.BlockCopy(decodedHashedPassword, 13 + salt.Length, expectedSubkey, 0, expectedSubkey.Length);

      // Hash the incoming password and verify it
      var actualSubkey = KeyDerivation.Pbkdf2(providedPassword, salt, prf, iterCount, subkeyLength);
      return actualSubkey.SequenceEqual(expectedSubkey);
    }

    public string Encode<T>(T payload, string secret)
    {
      var header = new {typ = "jwt", alg = "HMACSHA256" };

      var jwt = $"{ObjectToString(JsonConvert.SerializeObject(header))}.{ObjectToString(JsonConvert.SerializeObject(payload))}";

      HMACSHA256 hmacsha256 = new HMACSHA256(Encoding.ASCII.GetBytes(secret));
      byte[] result = hmacsha256.ComputeHash(Encoding.ASCII.GetBytes(jwt));

      return $"{jwt}.{Convert.ToBase64String(result)}";
    }

    public string Decode(string[] segments, string secret)
    {
      Payload payload = JsonConvert.DeserializeObject<Payload>(StringToObject<string>(segments[1]));
      string signature = this.Encode(payload, secret);
      return signature;
    }

    private static string ObjectToString<T>(T obj)
    {
      //string payload = JsonConvert.SerializeObject(obj);

      using (MemoryStream ms = new MemoryStream())
      {
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(ms, obj);
        return Convert.ToBase64String(ms.ToArray());
      }
    }

    private static T StringToObject<T>(string base64)
    {
      byte[] buffer = Convert.FromBase64String(base64);
      using (MemoryStream ms = new MemoryStream())
      {
        ms.Write(buffer, 0, buffer.Length);
        ms.Position = 0;
        return (T) new BinaryFormatter().Deserialize(ms);
      }
    }

    private static void WriteNetworkByteOrder(byte[] buffer, int offset, uint value)
    {
      buffer[offset + 0] = (byte)(value >> 24);
      buffer[offset + 1] = (byte)(value >> 16);
      buffer[offset + 2] = (byte)(value >> 8);
      buffer[offset + 3] = (byte)(value >> 0);
    }

    private static uint ReadNetworkByteOrder(byte[] buffer, int offset)
    {
      return ((uint) buffer[offset + 0] << 24)
             | ((uint) buffer[offset + 1] << 16)
             | ((uint) buffer[offset + 2] << 8)
             | (uint)buffer[offset + 3];
    }
  }

  public class UserController : BaseApiController<User>
  {
    private readonly IUserRepository repository;
    private readonly IHashHelper hashHelper;

    public UserController(IUserRepository repository, IHashHelper hashHelper) : base(repository)
    {
      this.repository = repository;
      this.hashHelper = hashHelper;
    }

    [AllowAnonymous]
    [Route("api/user/signin")]
    [HttpPost]
    public async Task<IHttpActionResult> Signin([FromBody] User user)
    {
      User savedUser = await repository.GetByName(user.Username);

      if (savedUser == null)
      {
        return NotFound();
      }

      bool result = hashHelper.VerifyHashedPassword(savedUser.Password, user.Password);

      if (!result)
      {
        return Unauthorized();
      }

      return SendResponse(savedUser);
    }

    [AllowAnonymous]
    [Route("api/user/signup")]
    [HttpPost]
    public async Task<IHttpActionResult> Register([FromBody] User user)
    {
      User savedUser = await this.repository.GetByName(user.Username);

      if (savedUser != null)
      {
        return BadRequest();
      }

      user.Password = hashHelper.HashPassword(user.Password);
      User updatedUser = await Repository.AddOrUpdate(null, user);
      updatedUser.Password = null;
      return SendResponse(updatedUser);
    }

    private IHttpActionResult SendResponse(User user)
    {
      user.Password = null;

      Payload payload = new Payload
      {
        iss = Request.RequestUri.Host,
        sub = user.Id
      };

      var token = hashHelper.Encode(payload, "secret");

      return Ok(new {user, token});
    }

  }
}

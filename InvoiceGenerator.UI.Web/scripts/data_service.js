;
(function() {
  'use strict';

  var module = angular.module('main_page');

  module.factory('dataService', ['$http', '$q', 'authToken',
    function($http, $q, authToken) {

      var baseUri = '/invoicegenerator.api/api/';

      function getEntities(source, uriPart) {
        var deferred = $q.defer();

        if (!authToken.isAuthenticated()) {
          deferred.reject({
            status: 'Authorization',
            statusText: 'token missing, data not saved!'
          });
          return deferred.promise;
        };

        $http({
          url: baseUri + uriPart,
          method: 'GET',
          headers: {
            'Authorization': 'Bearer ' + authToken.getToken().token
          }
        }).then(
          function(result) {
            if (result.data && result.data.length > 0) {
              if (source) {
                angular.copy(result.data, source);
              }
              deferred.resolve(result.data);
            } else {
              source = [];
              deferred.resolve(source);
            }
          },
          function(error) {
            deferred.reject(error.data.message);
          });

        return deferred.promise;
      }

      var basepath = 'templates/';

      var tabs = [{
        'id': 1,
        'name': 'Companies',
        'fontIcon': 'university',
        'view_template': basepath + 'company/companies.html',
        'edit_template': basepath + 'company/create_company.html',
        'sortColumn': 'name',
        'uri': 'company'
      }, {
        'id': 2,
        'name': 'Customers',
        'fontIcon': 'users',
        'view_template': basepath + '/customer/customers.html',
        'edit_template': basepath + 'customer/create_customer.html',
        'sortColumn': 'name',
        'uri': 'customer'
      }, {
        'id': 3,
        'name': 'Products',
        'fontIcon': 'tags',
        'view_template': basepath + 'product/products.html',
        'edit_template': basepath + 'product/create_product.html',
        'sortColumn': 'name',
        'uri': 'product'
      }, {
        'id': 4,
        'name': 'Invoices',
        'fontIcon': 'file-text',
        'view_template': basepath + 'invoice/invoices.html',
        'edit_template': basepath + 'invoice/create_invoice.html',
        'sortColumn': 'invoiceDate',
        'uri': 'invoice'
      }, {
        'id': 5,
        'name': 'Payments',
        'fontIcon': 'money',
        'view_template': basepath + 'bill/payments.html',
        'edit_template': basepath + 'bill/create_payment.html',
        'sortColumn': 'paymentDate',
        'uri': 'payment'
      }];

      var frequencies = [{
        id: 0,
        name: 'Other'
      }, {
        id: 1,
        name: 'Monthly'
      }, {
        id: 3,
        name: 'Quarterly'
      }, {
        id: 6,
        name: 'Halfyearly'
      }, {
        id: 12,
        name: 'Yearly'
      }, ];

      var countries = [{
        "id": "AF",
        "text": "Afghanistan"
      }, {
        "id": "AX",
        "text": "Åland Islands"
      }, {
        "id": "AL",
        "text": "Albania"
      }, {
        "id": "DZ",
        "text": "Algeria"
      }, {
        "id": "AS",
        "text": "American Samoa"
      }, {
        "id": "AD",
        "text": "Andorra"
      }, {
        "id": "AO",
        "text": "Angola"
      }, {
        "id": "AI",
        "text": "Anguilla"
      }, {
        "id": "AQ",
        "text": "Antarctica"
      }, {
        "id": "AG",
        "text": "Antigua and Barbuda"
      }, {
        "id": "AR",
        "text": "Argentina"
      }, {
        "id": "AM",
        "text": "Armenia"
      }, {
        "id": "AW",
        "text": "Aruba"
      }, {
        "id": "AU",
        "text": "Australia"
      }, {
        "id": "AT",
        "text": "Austria"
      }, {
        "id": "AZ",
        "text": "Azerbaijan"
      }, {
        "id": "BS",
        "text": "Bahamas"
      }, {
        "id": "BH",
        "text": "Bahrain"
      }, {
        "id": "BD",
        "text": "Bangladesh"
      }, {
        "id": "BB",
        "text": "Barbados"
      }, {
        "id": "BY",
        "text": "Belarus"
      }, {
        "id": "BE",
        "text": "Belgium"
      }, {
        "id": "BZ",
        "text": "Belize"
      }, {
        "id": "BJ",
        "text": "Benin"
      }, {
        "id": "BM",
        "text": "Bermuda"
      }, {
        "id": "BT",
        "text": "Bhutan"
      }, {
        "id": "BO",
        "text": "Bolivia"
      }, {
        "id": "BQ",
        "text": "Bonaire"
      }, {
        "id": "BA",
        "text": "Bosnia and Herzegovina"
      }, {
        "id": "BW",
        "text": "Botswana"
      }, {
        "id": "BV",
        "text": "Bouvet Island"
      }, {
        "id": "BR",
        "text": "Brazil"
      }, {
        "id": "IO",
        "text": "British Indian Ocean Territory"
      }, {
        "id": "VG",
        "text": "British Virgin Islands"
      }, {
        "id": "BN",
        "text": "Brunei"
      }, {
        "id": "BG",
        "text": "Bulgaria"
      }, {
        "id": "BF",
        "text": "Burkina Faso"
      }, {
        "id": "BI",
        "text": "Burundi"
      }, {
        "id": "KH",
        "text": "Cambodia"
      }, {
        "id": "CM",
        "text": "Cameroon"
      }, {
        "id": "CA",
        "text": "Canada"
      }, {
        "id": "CV",
        "text": "Cape Verde"
      }, {
        "id": "KY",
        "text": "Cayman Islands"
      }, {
        "id": "CF",
        "text": "Central African Republic"
      }, {
        "id": "TD",
        "text": "Chad"
      }, {
        "id": "CL",
        "text": "Chile"
      }, {
        "id": "CN",
        "text": "China"
      }, {
        "id": "CX",
        "text": "Christmas Island"
      }, {
        "id": "CC",
        "text": "Cocos (Keeling) Islands"
      }, {
        "id": "CO",
        "text": "Colombia"
      }, {
        "id": "KM",
        "text": "Comoros"
      }, {
        "id": "CG",
        "text": "Republic of the Congo"
      }, {
        "id": "CD",
        "text": "DR Congo"
      }, {
        "id": "CK",
        "text": "Cook Islands"
      }, {
        "id": "CR",
        "text": "Costa Rica"
      }, {
        "id": "HR",
        "text": "Croatia"
      }, {
        "id": "CU",
        "text": "Cuba"
      }, {
        "id": "CW",
        "text": "Curaçao"
      }, {
        "id": "CY",
        "text": "Cyprus"
      }, {
        "id": "CZ",
        "text": "Czech Republic"
      }, {
        "id": "DK",
        "text": "Denmark"
      }, {
        "id": "DJ",
        "text": "Djibouti"
      }, {
        "id": "DM",
        "text": "Dominica"
      }, {
        "id": "DO",
        "text": "Dominican Republic"
      }, {
        "id": "EC",
        "text": "Ecuador"
      }, {
        "id": "EG",
        "text": "Egypt"
      }, {
        "id": "SV",
        "text": "El Salvador"
      }, {
        "id": "GQ",
        "text": "Equatorial Guinea"
      }, {
        "id": "ER",
        "text": "Eritrea"
      }, {
        "id": "EE",
        "text": "Estonia"
      }, {
        "id": "ET",
        "text": "Ethiopia"
      }, {
        "id": "FK",
        "text": "Falkland Islands"
      }, {
        "id": "FO",
        "text": "Faroe Islands"
      }, {
        "id": "FJ",
        "text": "Fiji"
      }, {
        "id": "FI",
        "text": "Finland"
      }, {
        "id": "FR",
        "text": "France"
      }, {
        "id": "GF",
        "text": "French Guiana"
      }, {
        "id": "PF",
        "text": "French Polynesia"
      }, {
        "id": "TF",
        "text": "French Southern and Antarctic Lands"
      }, {
        "id": "GA",
        "text": "Gabon"
      }, {
        "id": "GM",
        "text": "Gambia"
      }, {
        "id": "GE",
        "text": "Georgia"
      }, {
        "id": "DE",
        "text": "Germany"
      }, {
        "id": "GH",
        "text": "Ghana"
      }, {
        "id": "GI",
        "text": "Gibraltar"
      }, {
        "id": "GR",
        "text": "Greece"
      }, {
        "id": "GL",
        "text": "Greenland"
      }, {
        "id": "GD",
        "text": "Grenada"
      }, {
        "id": "GP",
        "text": "Guadeloupe"
      }, {
        "id": "GU",
        "text": "Guam"
      }, {
        "id": "GT",
        "text": "Guatemala"
      }, {
        "id": "GG",
        "text": "Guernsey"
      }, {
        "id": "GN",
        "text": "Guinea"
      }, {
        "id": "GW",
        "text": "Guinea-Bissau"
      }, {
        "id": "GY",
        "text": "Guyana"
      }, {
        "id": "HT",
        "text": "Haiti"
      }, {
        "id": "HM",
        "text": "Heard Island and McDonald Islands"
      }, {
        "id": "VA",
        "text": "Vatican City"
      }, {
        "id": "HN",
        "text": "Honduras"
      }, {
        "id": "HK",
        "text": "Hong Kong"
      }, {
        "id": "HU",
        "text": "Hungary"
      }, {
        "id": "IS",
        "text": "Iceland"
      }, {
        "id": "IN",
        "text": "India"
      }, {
        "id": "ID",
        "text": "Indonesia"
      }, {
        "id": "CI",
        "text": "Ivory Coast"
      }, {
        "id": "IR",
        "text": "Iran"
      }, {
        "id": "IQ",
        "text": "Iraq"
      }, {
        "id": "IE",
        "text": "Ireland"
      }, {
        "id": "IM",
        "text": "Isle of Man"
      }, {
        "id": "IL",
        "text": "Israel"
      }, {
        "id": "IT",
        "text": "Italy"
      }, {
        "id": "JM",
        "text": "Jamaica"
      }, {
        "id": "JP",
        "text": "Japan"
      }, {
        "id": "JE",
        "text": "Jersey"
      }, {
        "id": "JO",
        "text": "Jordan"
      }, {
        "id": "KZ",
        "text": "Kazakhstan"
      }, {
        "id": "KE",
        "text": "Kenya"
      }, {
        "id": "KI",
        "text": "Kiribati"
      }, {
        "id": "KW",
        "text": "Kuwait"
      }, {
        "id": "KG",
        "text": "Kyrgyzstan"
      }, {
        "id": "LA",
        "text": "Laos"
      }, {
        "id": "LV",
        "text": "Latvia"
      }, {
        "id": "LB",
        "text": "Lebanon"
      }, {
        "id": "LS",
        "text": "Lesotho"
      }, {
        "id": "LR",
        "text": "Liberia"
      }, {
        "id": "LY",
        "text": "Libya"
      }, {
        "id": "LI",
        "text": "Liechtenstein"
      }, {
        "id": "LT",
        "text": "Lithuania"
      }, {
        "id": "LU",
        "text": "Luxembourg"
      }, {
        "id": "MO",
        "text": "Macau"
      }, {
        "id": "MK",
        "text": "Macedonia"
      }, {
        "id": "MG",
        "text": "Madagascar"
      }, {
        "id": "MW",
        "text": "Malawi"
      }, {
        "id": "MY",
        "text": "Malaysia"
      }, {
        "id": "MV",
        "text": "Maldives"
      }, {
        "id": "ML",
        "text": "Mali"
      }, {
        "id": "MT",
        "text": "Malta"
      }, {
        "id": "MH",
        "text": "Marshall Islands"
      }, {
        "id": "MQ",
        "text": "Martinique"
      }, {
        "id": "MR",
        "text": "Mauritania"
      }, {
        "id": "MU",
        "text": "Mauritius"
      }, {
        "id": "YT",
        "text": "Mayotte"
      }, {
        "id": "MX",
        "text": "Mexico"
      }, {
        "id": "FM",
        "text": "Micronesia"
      }, {
        "id": "MD",
        "text": "Moldova"
      }, {
        "id": "MC",
        "text": "Monaco"
      }, {
        "id": "MN",
        "text": "Mongolia"
      }, {
        "id": "ME",
        "text": "Montenegro"
      }, {
        "id": "MS",
        "text": "Montserrat"
      }, {
        "id": "MA",
        "text": "Morocco"
      }, {
        "id": "MZ",
        "text": "Mozambique"
      }, {
        "id": "MM",
        "text": "Myanmar"
      }, {
        "id": "NA",
        "text": "Namibia"
      }, {
        "id": "NR",
        "text": "Nauru"
      }, {
        "id": "NP",
        "text": "Nepal"
      }, {
        "id": "NL",
        "text": "Netherlands"
      }, {
        "id": "NC",
        "text": "New Caledonia"
      }, {
        "id": "NZ",
        "text": "New Zealand"
      }, {
        "id": "NI",
        "text": "Nicaragua"
      }, {
        "id": "NE",
        "text": "Niger"
      }, {
        "id": "NG",
        "text": "Nigeria"
      }, {
        "id": "NU",
        "text": "Niue"
      }, {
        "id": "NF",
        "text": "Norfolk Island"
      }, {
        "id": "KP",
        "text": "North Korea"
      }, {
        "id": "MP",
        "text": "Northern Mariana Islands"
      }, {
        "id": "NO",
        "text": "Norway"
      }, {
        "id": "OM",
        "text": "Oman"
      }, {
        "id": "PK",
        "text": "Pakistan"
      }, {
        "id": "PW",
        "text": "Palau"
      }, {
        "id": "PS",
        "text": "Palestine"
      }, {
        "id": "PA",
        "text": "Panama"
      }, {
        "id": "PG",
        "text": "Papua New Guinea"
      }, {
        "id": "PY",
        "text": "Paraguay"
      }, {
        "id": "PE",
        "text": "Peru"
      }, {
        "id": "PH",
        "text": "Philippines"
      }, {
        "id": "PN",
        "text": "Pitcairn Islands"
      }, {
        "id": "PL",
        "text": "Poland"
      }, {
        "id": "PT",
        "text": "Portugal"
      }, {
        "id": "PR",
        "text": "Puerto Rico"
      }, {
        "id": "QA",
        "text": "Qatar"
      }, {
        "id": "XK",
        "text": "Kosovo"
      }, {
        "id": "RE",
        "text": "Réunion"
      }, {
        "id": "RO",
        "text": "Romania"
      }, {
        "id": "RU",
        "text": "Russia"
      }, {
        "id": "RW",
        "text": "Rwanda"
      }, {
        "id": "BL",
        "text": "Saint Barthélemy"
      }, {
        "id": "SH",
        "text": "Saint Helena, Ascension and Tristan da Cunha"
      }, {
        "id": "KN",
        "text": "Saint Kitts and Nevis"
      }, {
        "id": "LC",
        "text": "Saint Lucia"
      }, {
        "id": "MF",
        "text": "Saint Martin"
      }, {
        "id": "PM",
        "text": "Saint Pierre and Miquelon"
      }, {
        "id": "VC",
        "text": "Saint Vincent and the Grenadines"
      }, {
        "id": "WS",
        "text": "Samoa"
      }, {
        "id": "SM",
        "text": "San Marino"
      }, {
        "id": "ST",
        "text": "São Tomé and Príncipe"
      }, {
        "id": "SA",
        "text": "Saudi Arabia"
      }, {
        "id": "SN",
        "text": "Senegal"
      }, {
        "id": "RS",
        "text": "Serbia"
      }, {
        "id": "SC",
        "text": "Seychelles"
      }, {
        "id": "SL",
        "text": "Sierra Leone"
      }, {
        "id": "SG",
        "text": "Singapore"
      }, {
        "id": "SX",
        "text": "Sint Maarten"
      }, {
        "id": "SK",
        "text": "Slovakia"
      }, {
        "id": "SI",
        "text": "Slovenia"
      }, {
        "id": "SB",
        "text": "Solomon Islands"
      }, {
        "id": "SO",
        "text": "Somalia"
      }, {
        "id": "ZA",
        "text": "South Africa"
      }, {
        "id": "GS",
        "text": "South Georgia"
      }, {
        "id": "KR",
        "text": "South Korea"
      }, {
        "id": "SS",
        "text": "South Sudan"
      }, {
        "id": "ES",
        "text": "Spain"
      }, {
        "id": "LK",
        "text": "Sri Lanka"
      }, {
        "id": "SD",
        "text": "Sudan"
      }, {
        "id": "SR",
        "text": "Suriname"
      }, {
        "id": "SJ",
        "text": "Svalbard and Jan Mayen"
      }, {
        "id": "SZ",
        "text": "Swaziland"
      }, {
        "id": "SE",
        "text": "Sweden"
      }, {
        "id": "CH",
        "text": "Switzerland"
      }, {
        "id": "SY",
        "text": "Syria"
      }, {
        "id": "TW",
        "text": "Taiwan"
      }, {
        "id": "TJ",
        "text": "Tajikistan"
      }, {
        "id": "TZ",
        "text": "Tanzania"
      }, {
        "id": "TH",
        "text": "Thailand"
      }, {
        "id": "TL",
        "text": "Timor-Leste"
      }, {
        "id": "TG",
        "text": "Togo"
      }, {
        "id": "TK",
        "text": "Tokelau"
      }, {
        "id": "TO",
        "text": "Tonga"
      }, {
        "id": "TT",
        "text": "Trinidad and Tobago"
      }, {
        "id": "TN",
        "text": "Tunisia"
      }, {
        "id": "TR",
        "text": "Turkey"
      }, {
        "id": "TM",
        "text": "Turkmenistan"
      }, {
        "id": "TC",
        "text": "Turks and Caicos Islands"
      }, {
        "id": "TV",
        "text": "Tuvalu"
      }, {
        "id": "UG",
        "text": "Uganda"
      }, {
        "id": "UA",
        "text": "Ukraine"
      }, {
        "id": "AE",
        "text": "United Arab Emirates"
      }, {
        "id": "GB",
        "text": "United Kingdom"
      }, {
        "id": "US",
        "text": "United States"
      }, {
        "id": "UM",
        "text": "United States Minor Outlying Islands"
      }, {
        "id": "VI",
        "text": "United States Virgin Islands"
      }, {
        "id": "UY",
        "text": "Uruguay"
      }, {
        "id": "UZ",
        "text": "Uzbekistan"
      }, {
        "id": "VU",
        "text": "Vanuatu"
      }, {
        "id": "VE",
        "text": "Venezuela"
      }, {
        "id": "VN",
        "text": "Vietnam"
      }, {
        "id": "WF",
        "text": "Wallis and Futuna"
      }, {
        "id": "EH",
        "text": "Western Sahara"
      }, {
        "id": "YE",
        "text": "Yemen"
      }, {
        "id": "ZM",
        "text": "Zambia"
      }, {
        "id": "ZW",
        "text": "Zimbabwe"
      }];

      var companies = [];
      var companiesLoaded = false;

      var getCompanies = function(reload) {
        if (!companiesLoaded || reload) {
          companiesLoaded = true;
          return getEntities(companies, 'company');
        }
        return $q.resolve(companies);
      }

      var customers = [];
      var customersLoaded = false;

      var getCustomers = function(reload) {
        if (!customersLoaded || reload) {
          customersLoaded = true;
          return getEntities(customers, 'customer');
        }
        return $q.resolve(customers);
      }

      var products = [];
      var productsLoaded = false;

      var getProducts = function(reload) {
        if (!productsLoaded || reload) {
          productsLoaded = true;
          return getEntities(products, 'product');
        }
        return $q.resolve(products);
      }

      var invoices = [];
      var invoicesLoaded = false;
      var getInvoices = function(reload) {
        if (!invoicesLoaded || reload) {
          invoicesLoaded = true;
          return getEntities(invoices, 'invoice');
        }
        return $q.resolve(invoices);
      }

      var payments = [];
      var paymentsLoaded = false;

      var getPayments = function(reload) {
        if (!paymentsLoaded || reload) {
          paymentsLoaded = true;
          return getEntities(payments, 'payment');
        }
        return $q.resolve(payments);
      }

      var saveEntity = function(entity, entityName, token) {
        var deferred = $q.defer();

        if (token === undefined) {
          deferred.reject({
            status: 'Authorization',
            statusText: 'token missing, data not saved!'
          });
          return deferred.promise;
        };

        $http({
            url: entity.id ? baseUri + entityName + '/' + entity.id : baseUri + entityName,
            method: entity.id ? 'PUT' : 'POST',
            data: entity,
            headers: {
              'Authorization': 'Bearer ' + authToken.getToken().token
            }
          })
          .then(function(data) {
              //console.log(data);
              deferred.resolve(data);
            },
            function(data) {
              //console.log(data);
              deferred.reject(data);
            });

        return deferred.promise;
      };

      var signinsignout = function(entity, entityName) {
        var deferred = $q.defer();

        $http({
            url: entity.id ? baseUri + entityName + '/' + entity.id : baseUri + entityName,
            method: 'POST',
            data: entity
          })
          .then(function(data) {
              //console.log(data);
              deferred.resolve(data);
            },
            function(data) {
              //console.log(data);
              deferred.reject(data);
            });

        return deferred.promise;
      };

      function downloadPdf(uriPart) {
        var deferred = $q.defer();

        $http({
          method: 'GET',
          url: baseUri + uriPart,
          headers: {
            'Authorization': 'Bearer ' + authToken.getToken().token
          },
          responseType: 'arraybuffer'
        });

        return deferred.promise;
      }

      var getEntity = function(entityName, reload) {
        switch (entityName) {
          case 1:
            return getCompanies(reload);
          case 2:
            return getCustomers(reload);
          case 3:
            return getProducts(reload);
          case 4:
            return getInvoices(reload);
          case 5:
            return getPayments(reload);
          case 6: //http://localhost/invoicegenerator.api/api/invoice/pdf/57c3df85e1088c24581de569
            return downloadPdf('invoice/pdf/' + reload);
          case 7: //http://localhost/invoicegenerator.api/api/company/logo/57c3df85e1088c24581de569
            return downloadPdf('company/logo/' + reload);
        }
        return null;
      }

      // var getIcon = function(ctrl) {
      //     if (!ctrl) {
      //         return null;
      //     }

      //     if (ctrl.$invalid || !ctrl.$valid) {
      //         return 'fa fa-times text-danger';
      //     } else if (ctrl.$valid) {
      //         return 'fa fa-check text-success';
      //     }
      // }

      var getMsg = function(ctrl, msg, length, type) {
        if (!ctrl) {
          return null;
        }

        if (ctrl.$error.required) {
          return msg + ' is required.';
        }

        if (ctrl.$error.maxlength) {
          if (length)
            return msg + ' exceeding max length ' + length + ' allowed.';
          else
            return msg + ' exceeding max length allowed.';
        }


        if (ctrl.$error.pattern) {
          return 'Please enter valid ' + msg + '.';
        }

        if (ctrl.$valid) {
          if (ctrl.$name === 'comments') {
            return msg + ' are valid.';
          }
          return msg + ' is valid.';
        }
        return null;
      }

      var notifyStatus = function(notification) {
        $.notify({
          icon: 'fa fa-lg ' + notification.icon,
          title: notification.title,
          message: notification.message
        }, {
          element: 'body',
          type: notification.type,
          allow_dismiss: true,
          placement: {
            from: 'bottom',
            align: 'right'
          },
          timer: 2000,
          animate: {
            enter: 'animated fadeInDown',
            exit: 'animated fadeOutUp'
          },
          template: '<div data-notify="container" class="modal-slg col-xs-11 col-sm-3 alert alert-{0}" role="alert"><span data-notify="title"><strong>{1}</strong></span><hr class="message-inner-separator"><span data-notify="icon"></span> <span data-notify="message">{2}</span></div>'
        });
      }

      var notifySuccess = function(notification) {
        notifyStatus({
          title: notification.title,
          message: notification.message,
          icon: 'fa-check',
          type: 'success'
        });
      }

      var notifyError = function(notification) {
        notifyStatus({
          title: notification.title,
          message: notification.message,
          icon: 'fa-bell',
          type: 'danger'
        });
      }

      var notifyWarning = function(notification) {
        notifyStatus({
          title: notification.title,
          message: notification.message,
          icon: 'fa-exclamation',
          type: 'warning'
        });
      }

      var text = /^[a-zA-Z0-9\s.\-,@]+$/;
      var decimal = /^\d*(\.\d+)?$/;
      var email = /^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+(?:[A-Z]{2}|com|org|net|gov|mil|biz|info|mobi|name|aero|jobs|museum|in|co\.in|edu)\b$/;
      var phone = /^(\+\d{1,3} )?\d{2,4}[\- ]?\d{3,4}[\- ]?\d{4}$/;
      var zip = /^\d{5,6}([\-]?\d{4})?$/;
      var percent = /^(100(?:\.0{1,2})?|0{0,2}?\.[\d]{1,2}|[\d]{1,2}(?:\.[\d]{1,2})?)$/;
      var integer = /^\d{1,6}$/;

      var dateFormat = 'EEEE, MMMM dd, yyyy';
      var shortDateFormat = 'EEE, MMM dd, yyyy';

      var dateOptions = {
        startingDay: 1,
        showWeeks: false
      };

      return {
        tabs: tabs,
        getEntity: getEntity,
        //downloadPdf: downloadPdf,
        saveEntity: saveEntity,
        frequencies: frequencies,
        countries: countries,
        getMessage: getMsg,
        text: text,
        decimal: decimal,
        email: email,
        phone: phone,
        zip: zip,
        percent: percent,
        integer: integer,
        dateFormat: dateFormat,
        shortDateFormat: shortDateFormat,
        dateOptions: dateOptions,
        notifySuccess: notifySuccess,
        notifyError: notifyError,
        notifyWarning: notifyWarning,
        signinsignout: signinsignout
      };
    }
  ]);
}());
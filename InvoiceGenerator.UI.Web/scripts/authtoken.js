;
(function() {
  'use strict';

  angular.module('main_page').factory('authToken', function($window) {
    var storage = $window.localStorage;
    var key = 'userToken';
    var cachedToken;

    var authToken = {
      setToken: function(token) {
        cachedToken = token;
        storage.setItem(key, JSON.stringify(token));
      },
      getToken: function() {
        if (!cachedToken) {
          cachedToken = JSON.parse(storage.getItem(key));
        }
        return cachedToken;
      },
      isAuthenticated: function() {
        return !!authToken.getToken();
      },
      removeToken: function() {
        cachedToken = null;
        storage.removeItem(key);
      }
    };
    return authToken;
  });

}());
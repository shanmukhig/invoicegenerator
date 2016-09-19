;
(function() {
  'use strict';
  var module = angular.module('main_page');

  module.controller('customerController', ['$scope', '$uibModalInstance', 'modal', 'dataService', 'authToken', function($scope, $uibModalInstance, modal, dataService, authToken) {

    $scope.entity = modal.entity;
    $scope.tabName = modal.entityName;
    $scope.ds = dataService;
    $scope.tabId = 1;
    $scope.tax = {};

    $scope.getActive = function(item) {
      return item === parseInt($scope.tabId);
    }

    $scope.close = function() {
      dataService.notifyWarning({
        title: 'Close dialog',
        message: 'All the chagnes are discarded'
      });
      $uibModalInstance.dismiss("user closed the dialog");
    }

    $scope.save = function() {
      $scope.isBusy = true;
      $scope.entity.currency = $('input[name="currency-picker"]').val();

      dataService.saveEntity($scope.entity, $scope.tabName, authToken.getToken().token)
        .then(function() {

            dataService.notifySuccess({
              title: 'Customer details',
              message: $scope.entity.id ? 'Customer details updated successfully' : 'Customer created successfully'
            });

            $scope.entity.taxHtml = '';
            $scope.entity.commHtml = '';
            $scope.entity.addressHtml = '';

            $uibModalInstance.close();
          },
          function(reason) {
            console.log(reason);
            dataService.notifyError({
              title: 'Customer details',
              message: reason.status + ' ' + reason.statusText
            });
          })
        .finally(function() {
          $scope.isBusy = false;
        });
    }

    $scope.addTax = function() {
      if ($scope.tax.name == undefined || $scope.tax.percent == undefined) {
        dataService.notifyWarning({
          title: 'Add tax',
          message: 'Please fill in all the tax information'
        });
        return;
      }

      var tax = {};
      angular.copy($scope.tax, tax);
      if ($scope.entity.taxes == undefined) {
        $scope.entity.taxes = [];
      }
      $scope.entity.taxes.push(tax);
      $scope.tax = {};
    }

    $scope.deleteTax = function(item) {
      var index = $scope.entity.taxes.indexOf(item);
      $scope.entity.taxes.splice(index, 1);
    }
  }]);
})();
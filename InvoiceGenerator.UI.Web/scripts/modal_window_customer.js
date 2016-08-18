(function() {
  var module = angular.module('main_page');

  module.controller('customerController', ['$scope', '$uibModalInstance', 'modal', 'dataService', function($scope, $uibModalInstance, modal, dataService) {
    $scope.modal = modal;

    $scope.entity = modal.entity;

    $scope.ds = dataService;

    $scope.entity.taxHtml = '';
    $scope.entity.commHtml = '';
    $scope.entity.addressHtml = '';

    $scope.tabId = 1;

    $scope.countries = dataService.countries;

    $scope.tax = {};

    $scope.getActive = function(item) {
      return item === parseInt($scope.tabId);
    }

    $scope.close = function() {
      var entity = $scope.entity;
      $uibModalInstance.dismiss("user closed the dialog");
    }

    $scope.save = function() {

      var ctrl = $('input[name="currency-picker"]');
      if (ctrl && ctrl.length > 0) {
        if ($('button[name="currency-picker"]').hasClass('ng-touched')) {
          $scope.entity.currency = ctrl.val();
        }
      }

      $uibModalInstance.close({
        entity: $scope.entity,
        entityName: modal.entityName
      });
    }

    $scope.addTax = function() {
      var tax = {};
      angular.copy($scope.tax, tax);
      if (!$scope.entity.taxes) {
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
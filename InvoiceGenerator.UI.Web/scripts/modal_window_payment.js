(function() {
  var module = angular.module('main_page');

  module.controller('paymentController', ['$scope', '$uibModalInstance', 'modal', 'dataService', '$filter', function($scope, $uibModalInstance, modal, dataService, $filter) {

    $scope.entity = modal.entity;

    $scope.ds = dataService;

    dataService.getEntity(dataService.tabs[3].id, false).then(function(data) {
      $scope.invoices = data;
    });

    $scope.backup = {};
    angular.copy($scope.entity, $scope.backup);

    $scope.setCurrency = function() {
      var invoice = $filter('filter')($scope.invoices, {
        id: $scope.entity.invoiceNo
      });

      if (!invoice) {
        return;
      }

      $scope.entity.currency = invoice.currency;
    }

    $scope.close = function() {
      angular.copy($scope.backup, $scope.entity);
      $uibModalInstance.dismiss("user closed the dialog");
    }

    $scope.save = function() {
      var ctrl = $('input[name="currency-picker"]');
      if (ctrl && ctrl.length > 0) {
        $scope.entity.currency = $('input[name="currency-picker"]').val();
      }

      $uibModalInstance.close({
        entity: $scope.entity,
      });
    }

  }]);
})();
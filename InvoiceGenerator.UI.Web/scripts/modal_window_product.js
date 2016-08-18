(function() {
  var module = angular.module('main_page');

  module.controller('productController', ['$scope', '$uibModalInstance', 'modal', 'dataService', function($scope, $uibModalInstance, modal, dataService) {

    $scope.entity = modal.entity;

    $scope.countries = dataService.countries;

    $scope.backup = {};
    angular.copy($scope.entity, $scope.backup);

    $scope.close = function() {
      angular.copy($scope.backup, $scope.entity);
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
      });
    }

  }]);
})();
(function() {
  var module = angular.module('main_page');

  module.controller('companyController', ['$scope', '$uibModalInstance', 'modal', 'dataService', function($scope, $uibModalInstance, modal, dataService) {

    $scope.modal = modal;

    $scope.entity = modal.entity;

    $scope.entity.companyTaxesHtml = '';

    $scope.countries = dataService.countries;

    $scope.ds = dataService;

    $scope.tabId = 1;

    $scope.getActive = function(item) {
      return item === parseInt($scope.tabId);
    }

    $scope.convertToBase64 = function($file) {
      if (!$file) {
        return;
      }

      var fr = new FileReader()
      fr.onloadend = function(e) {
        $scope.entity.logo = e.target.result;
        $scope.$apply();
      }

      fr.readAsDataURL($file);
    };

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
        //entityName: modal.entityName
      });
    }
  }]);
})();
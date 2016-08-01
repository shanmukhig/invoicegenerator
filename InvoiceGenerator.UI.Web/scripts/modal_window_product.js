(function() {
  var module = angular.module('main_page');

  module.controller('productController', ['$scope', '$uibModalInstance', 'modal', function($scope, $uibModalInstance, modal) {

    $scope.text = /^[a-zA-Z0-9\s.\-,@]+$/;
    $scope.decimal = /^\d*(\.\d+)?$/;

    $scope.entity = modal.entity;

    $scope.backup = {};
    angular.copy($scope.entity, $scope.backup);

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

    $scope.getIcon = function(ctrl) {
      if (!ctrl) {
        return null;
      }

      if (ctrl.$invalid || !ctrl.$valid) {
        return 'fa fa-times text-danger';
      } else if (ctrl.$valid) {
        return 'fa fa-check text-success';
      }

      return null;
    }

    $scope.getMsg = function(ctrl, msg) {
      if (!ctrl) {
        return null;
      }

      if (ctrl.$name === 'logo') {
        if ($scope.entity.logo && $scope.entity.logo === '') {
          return 'Company logo is required';
        } else {
          return 'Company logo is valid';
        }
      }

      if (ctrl.$error.pattern) {
        return 'Please enter valid ' + msg + '.';
      }

      var text = '';

      if (ctrl.$error.required) {
        text = ' is required.';
      }

      if (ctrl.$error.maxlength) {
        text = ' exceeding max length allowed.';
      }

      if (ctrl.$valid) {
        if (ctrl.$name === 'comments') {
          text = ' are valid.';
        } else {
          text = ' is valid.';
        }
      }

      return msg + text;
    };
  }]);
})();
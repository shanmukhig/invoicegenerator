;
(function() {
  'use strict';

  var module = angular.module('main_page');

  module.controller('productController',
  ['$scope', '$uibModalInstance', 'modal', 'dataService',
    function($scope, $uibModalInstance, modal, dataService) {

      $scope.entity = modal.entity;
      $scope.tabName = modal.entityName;
      $scope.ds = dataService;

      $scope.close = function() {
        dataService.notifyWarning({
          title: 'Close dialog',
          message: 'All the chagnes are discarded'
        });
        $uibModalInstance.dismiss("user closed the dialog");
      }

      $scope.save = function() {
        $scope.isBusy = true;

        dataService.saveEntity($scope.entity, $scope.tabName)
          .then(function() {

              dataService.notifySuccess({
                title: 'Product details',
                message: $scope.entity.id ? 'Product details updated successfully' : 'Product created successfully'
              });

              $uibModalInstance.close();
            },
            function(reason) {
              console.log(reason);
              dataService.notifyError({
                title: 'Product details',
                message: reason.status + ' ' + reason.statusText
              });
            })
          .finally(function() {
            $scope.isBusy = false;
          });
      }
    }
  ]);
})();
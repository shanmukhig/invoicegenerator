;
(function() {
  'use strict';
  var module = angular.module('main_page');

  module.controller('companyController',
  ['$scope', '$uibModalInstance', 'modal', 'dataService',
    function($scope, $uibModalInstance, modal, dataService) {

      $scope.entity = modal.entity;
      $scope.tabName = modal.entityName;

      $scope.ds = dataService;

      $scope.tabId = 1;

      $scope.getActive = function(item) {
        return item === parseInt($scope.tabId);
      }

      $scope.convertToBase64 = function($file) {
        if (!$file) {
          return;
        }

        var fr = new FileReader();
        fr.onloadend = function(e) {
          $scope.entity.logo = e.target.result;
          $scope.$apply();
        }

        fr.readAsDataURL($file);
      };

      $scope.close = function () {
        dataService.notifyWarning({
          title: 'Close dialog',
          message: 'All the changes are discarded'
        });
        $uibModalInstance.dismiss("user closed the dialog");
      }

      $scope.getBusyStatus = function() {
        return $scope.entity.logo == undefined;
      }

      $scope.save = function() {
        $scope.isBusy = true;
        $scope.entity.currency = $('input[name="currency-picker"]').val();
        dataService.saveEntity($scope.entity, $scope.tabName)
          .then(function() {
              dataService.notifySuccess({
                title: 'Company details',
                message: $scope.entity.id ? 'Company details updated successfully' : 'Company created successfully'
              });
              $scope.entity.companyTaxesHtml = '';
              $scope.entity.contaddressHtml = '';
              $scope.entity.commHtml = '';
              $scope.entity.chequeHtml = '';
              $scope.entity.swiftHtml = '';
              $scope.entity.ifscHtml = '';
              $scope.entity.supportHtml = '';

              $uibModalInstance.close();
            },
            function(reason) {
              console.log(reason);
              dataService.notifyError({
                title: 'Company details',
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
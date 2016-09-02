;
(function() {
  'use strict';

  var module = angular.module('main_page');

  module.controller('paymentController',
  ['$scope', '$uibModalInstance', 'modal', 'dataService', '$filter',
    function($scope, $uibModalInstance, modal, dataService, $filter) {

      if (modal.entity.id) {
        modal.entity.paymentDate = new Date(modal.entity.paymentDate);
      } else {
        modal.entity.paymentDate = new Date();
      }

      $scope.entity = modal.entity;
      $scope.tabName = modal.entityName;

      $scope.ds = dataService;

      dataService.getEntity(dataService.tabs[3].id, true)
        .then(function(data) {
          $scope.invoices = data;
        });

      $scope.setCurrency = function() {
        if ($scope.entity.invoiceId == undefined) {
          $scope.entity = {};
          return;
        }
        var invoice = $filter('filter')($scope.invoices,
        {
          id: $scope.entity.invoiceId
        });

        if (invoice.length === 0) {
          return;
        }

        $scope.entity.currency = invoice[0].currency;
        $scope.entity.amount = invoice[0].totalDue;
        $scope.entity.invoiceNo = invoice[0].invoiceNo;
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

        dataService.saveEntity($scope.entity, $scope.tabName)
          .then(function() {
              dataService.notifySuccess({
                title: 'Payment details',
                message: $scope.entity.id ? 'Payment details updated successfully' : 'Payment created successfully'
              });

              $uibModalInstance.close();
            },
            function(reason) {
              console.log(reason);
              dataService.notifyError({
                title: 'Payment details',
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
;
(function() {
  'use strict';

  var module = angular.module('main_page');

  module.controller('invoiceController',
  ['$scope', '$uibModalInstance', 'modal', 'dataService', '$filter', '$q',
    function($scope, $uibModalInstance, modal, dataService, $filter, $q) {
      var dt = new Date();
      if (modal.entity.id) {
        modal.entity.startDate = new Date(modal.entity.startDate);
        modal.entity.endDate = new Date(modal.entity.endDate);
        modal.entity.dueDate = new Date(modal.entity.dueDate);
        modal.entity.billDate = new Date(modal.entity.billDate);
      } else {
        modal.entity.startDate = dt;
        modal.entity.endDate = new Date(new Date().setDate(dt.getDate() + 30));
        modal.entity.dueDate = new Date(new Date().setDate(dt.getDate() + 15));
        modal.entity.billDate = new Date(new Date().setDate(dt.getDate() + 5));
      }

      $scope.getNewBill = function () {
        return { startDate: dt, endDate: new Date(new Date().setDate(dt.getDate() + 30)), billingFrequency: 1, quantity: 1 };
      }
      
      $scope.entity = modal.entity;
      $scope.tabName = modal.entityName;
      $scope.bill = $scope.getNewBill();

      $scope.ds = dataService;

      $scope.filteredProducts = [];
      $scope.filteredCustomers = [];

      //angular.copy($scope.entity, $scope.backup);

      $scope.setCountry = function () {

        if (!$scope.entity.companyId) {
          $scope.entity = {};
          return;
        }

        var company = $filter('filter')($scope.companies,
        {
          id: $scope.entity.companyId
        });

        if (!company) {
          return;
        }

        $scope.filteredCustomers = [];

        $.each($scope.customers,
          function (i, item) {
            if (item.address.countryCode === company[0].address.countryCode) {
              $scope.filteredCustomers.push(item);
            }
          });

        $scope.filteredProducts = [];

        $.each($scope.products,
          function (i, item) {
            if (item.countryCode === company[0].address.countryCode) {
              $scope.filteredProducts.push(item);
            }
          });

        $scope.entity.currency = company[0].currency;
      }

      $q.all([
          dataService.getEntity(dataService.tabs[0].id, false),
          dataService.getEntity(dataService.tabs[1].id, false),
          dataService.getEntity(dataService.tabs[2].id, false)
        ])
        .then(function(data) {
            $scope.companies = data[0];
            $scope.customers = data[1];
            $scope.products = data[2];

            $scope.tabId = 1;

            if ($scope.entity.id) {
              $scope.setCountry();
            }
          },
          function(reason) {
            console.log(reason);
          });

      $scope.getPrice = function() {
        $.each($scope.filteredProducts,
          function(i, item) {
            if (item.id === $scope.bill.productId) {
              $scope.bill.productName = item.productName;
              $scope.bill.price = item.price;
              return;
            }
          });
      }

      $scope.addProduct = function () {

        if (!$scope.bill.price ||
          !$scope.bill.productId ||
          !$scope.bill.startDate ||
          !$scope.bill.endDate ||
          !$scope.bill.quantity ||
          !$scope.bill.billingFrequency) {
          dataService.notifyWarning({
            title: 'Add product details',
            message: 'Please fill in all the product details'
          });
          return;
        }

        var bill = {};
        angular.copy($scope.bill, bill);
        $scope.bill = $scope.getNewBill();
        $('select[name="productName"]').selectedIndex = -1;
        if (!$scope.entity.bills) {
          $scope.entity.bills = [];
        }
        $scope.entity.bills.push(bill);
      }

      $scope.deleteProduct = function(index) {
        $scope.entity.bills.splice(index, 1);
      }

      $scope.getActive = function(item) {
        return item === parseInt($scope.tabId);
      }

      $scope.close = function () {
        dataService.notifyWarning({
          title: 'Close dialog',
          message: 'All the chagnes are discarded'
        });
        $uibModalInstance.dismiss("user closed the dialog");
      }

      $scope.save = function() {
        $scope.isBusy = true;

        dataService.saveEntity($scope.entity, $scope.tabName)
          .then(function () {
              dataService.notifySuccess({
                title: 'Invoice details',
                message: $scope.entity.id ? 'Invoice details updated successfully' : 'Invoice created successfully'
              });
              $uibModalInstance.close();
            },
            function (reason) {
              dataService.notifyError({
                title: 'Invoice Details',
                message: reason.status + ' ' + reason.statusText
              });
              console.log(reason);
            })
          .finally(function() {
            $scope.isBusy = false;
          });
      }
    }
  ]);
})();
(function() {
    var module = angular.module('main_page');

    module.controller('invoiceController', ['$scope', '$uibModalInstance', 'modal', 'dataService', '$filter', function($scope, $uibModalInstance, modal, dataService, $filter) {

        $scope.entity = modal.entity;
        $scope.bill = {};
        $scope.backup = {};

        $scope.filteredProducts = [];
        $scope.filteredCustomers = [];

        angular.copy($scope.entity, $scope.backup);

        dataService.getEntity(dataService.tabs[0].id, false).then(function(data) {
            $scope.companies = data;
        }, function(reason) {});

        dataService.getEntity(dataService.tabs[1].id, false).then(function(data) {
            $scope.customers = data;
        }, function(reason) {});

        dataService.getEntity(dataService.tabs[2].id, false).then(function(data) {
            $scope.products = data;
        }, function(reason) {});

        $scope.frequencies = dataService.frequencies;

        $scope.tabId = 1;

        $scope.getPrice = function() {
            $.each($scope.filteredProducts, function(i, item) {
                if (item.id === $scope.bill.productId) {
                    //$scope.bill.productId = item.id;
                    $scope.bill.name = item.productName;
                    $scope.bill.price = item.price;
                    $scope.bill.currency = item.currency;
                    //$scope.getTotal();
                    return;
                }
            });
        }

        $scope.setCountry = function() {

            var company = $filter('filter')($scope.companies, {
                id: $scope.entity.companyId
            });

            if (!company) {
                return;
            }

            $scope.filteredCustomers = [];

            $.each($scope.customers, function(i, item) {
                if (item.address.countryCode === company[0].address.countryCode) {
                    $scope.filteredCustomers.push(item);
                }
            });

            $scope.filteredProducts = [];

            $.each($scope.products, function(i, item) {
                if (item.countryCode === company[0].address.countryCode) {
                    $scope.filteredProducts.push(item);
                }
            });
        }

        $scope.addProduct = function() {
            var bill = {};
            angular.copy($scope.bill, bill);
            $scope.bill = {};
            $('select[name="productName"]').selectedIndex = -1;
            if (!$scope.entity.bills) {
                $scope.entity.bills = [];
            }
            $scope.entity.bills.push(bill);
        }

        $scope.deleteProduct = function(item) {
            var index = $scope.entity.bills.indexOf(item);
            $scope.entity.bills.splice(index, 1);
        }

        $scope.getActive = function(item) {
            return item === parseInt($scope.tabId);
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
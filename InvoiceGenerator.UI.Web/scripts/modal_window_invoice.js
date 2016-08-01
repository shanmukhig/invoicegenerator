(function() {
    var module = angular.module('main_page');

    module.controller('invoiceController', ['$scope', '$uibModalInstance', 'modal', 'dataService', function($scope, $uibModalInstance, modal, dataService) {

        $scope.percent = /^(100(?:\.0{1,2})?|0{0,2}?\.[\d]{1,2}|[\d]{1,2}(?:\.[\d]{1,2})?)$/;
        $scope.text = /^[a-zA-Z0-9\s.\-,@]+$/;
        $scope.integer = /^\d{1,6}$/;
        $scope.decimal = /^\d*(\.\d+)?$/;
        $scope.email = /^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+(?:[A-Z]{2}|com|org|net|gov|mil|biz|info|mobi|name|aero|jobs|museum|in|co\.in|edu)\b$/;
        $scope.phone = /^(\+\d{1,3} )?\d{2,4}[\- ]?\d{3,4}[\- ]?\d{4}$/;
        $scope.zip = /^\d{5,6}([\-]?\d{4})?$/;

        $scope.entity = modal.entity;
        $scope.bill = {};
        $scope.backup = {};
        angular.copy($scope.entity, $scope.backup);

        dataService.getEntity(dataService.tabs[0].id, false).then(function(data) {
            $scope.companies = data;
        }, function(reason) {});

        dataService.getEntity(dataService.tabs[1].id, false).then(function(data) {
            $scope.customers = data;
        }, function(reason) {});

        dataService.getEntity(dataService.tabs[2].id, false).then(function(data) {
            $scope.products = data
        }, function(reason) {});

        $scope.frequencies = dataService.frequencies;

        $scope.tabId = 1;

        $scope.getPrice = function(id) {
            $.each($scope.products, function(i, item) {
                if (item.id === id) {
                    $scope.bill.productId = item.id;
                    $scope.bill.name = item.productName;
                    $scope.bill.price = item.price;
                    $scope.bill.currency = item.currency;
                    $scope.getTotal();
                    return;
                }
            });
        }

        $scope.addProduct = function() {
            var bill = {};
            angular.copy($scope.bill, bill);
            $scope.bill = {};
            if (!$scope.entity.bills) {
                $scope.entity.bills = [];
            }
            $scope.entity.bills.push(bill);
        }

        $scope.getActive = function(item) {
            return item === parseInt($scope.tabId);
        }

        $scope.dateOptions = {
            formatYear: 'yyyy',
            maxDate: new Date(2020, 5, 22),
            minDate: new Date(1900, 1, 1),
            startingDay: 1
        };

        $scope.getTotal = function() {
            if ($scope.bill.price && $scope.bill.quantity) {
                $scope.bill.total = parseFloat($scope.bill.price) * parseFloat($scope.bill.quantity);
            } else {
                $scope.total = 0.00;
            }
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
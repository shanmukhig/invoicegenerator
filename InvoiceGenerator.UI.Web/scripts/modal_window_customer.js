(function() {
    var module = angular.module('main_page');

    module.controller('customerController', ['$scope', '$uibModalInstance', 'modal', function($scope, $uibModalInstance, modal) {
        $scope.modal = modal;

        $scope.text = /^[a-zA-Z0-9\s.\-,@]+$/;
        $scope.decimal = /^\d*(\.\d+)?$/;
        $scope.email = /^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+(?:[A-Z]{2}|com|org|net|gov|mil|biz|info|mobi|name|aero|jobs|museum|in|co\.in|edu)\b$/;
        $scope.phone = /^(\+\d{1,3} )?\d{2,4}[\- ]?\d{3,4}[\- ]?\d{4}$/;
        $scope.zip = "^\d{5,6}([\-]?\d{4})?$"

        $scope.entity = modal.entity;

        $scope.tabId = 1;

        $scope.getActive = function(item) {
            return item === parseInt($scope.tabId);
        }

        $scope.close = function() {
            var entity = $scope.entity;
            $uibModalInstance.dismiss("user closed the dialog");
        }

        $scope.save = function() {
            $uibModalInstance.close({
                entity: $scope.entity,
                entityName: modal.entityName
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
        }

        $scope.getMsg = function(ctrl, msg, length, type) {
            if (!ctrl) {
                return null;
            }

            if (ctrl.$error.required) {
                return msg + ' is required.';
            }

            if (ctrl.$error.maxlength) {
                return msg + ' exceeding max length ' + length + ' allowed.';
            }

            if (ctrl.$error.pattern) {
                return 'Please enter valid ' + msg + '.';
            }

            if (ctrl.$valid) {
                if (ctrl.$name === 'comments') {
                    return msg + ' are valid.'
                }
                return msg + ' is valid.'
            }
        };
    }]);
})();
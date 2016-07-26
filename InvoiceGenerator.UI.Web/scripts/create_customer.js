(function() {
    var module = angular.module('main_page', ['ngRoute', 'ui.bootstrap', 'ngFileUpload']);

    module.controller('mController', ['$scope', '$uibModalInstance', 'modal', function($scope, $uibModalInstance, modal) {
        $scope.modal = modal;

        //$scope.percent = /^(100(?:\.0{1,2})?|0{0,2}?\.[\d]{1,2}|[\d]{1,2}(?:\.[\d]{1,2})?)$/;
        $scope.text = /^[a-zA-Z0-9\s.\-,@]+$/;
        //$scope.integer = /^\d{1,6}$/;
        //$scope.decimal = /^\d*(\.\d+)?$/;
        $scope.email = /^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+(?:[A-Z]{2}|com|org|net|gov|mil|biz|info|mobi|name|aero|jobs|museum|in|co\.in|edu)\b$/;
        $scope.phone = /^(\+\d{1,3} )?\d{2,4}[\- ]?\d{3,4}[\- ]?\d{4}$/;
        $scope.zip = "^\d{5,6}([\-]?\d{4})?$"

        $scope.entity = modal.entity;
        //$scope.opened = false;
        //$scope.data = modal.scope.data;

        $scope.tabId = 1;

        $scope.getActive = function(item) {
            return item === parseInt($scope.tabId);
        }

        // $scope.dateOptions = {
        //     //dateDisabled: disabled,
        //     formatYear: 'yyyy',
        //     //maxDate: new Date(2020, 5, 22),
        //     minDate: new Date(1900, 1, 1),
        //     startingDay: 1
        // };

        // $scope.open = function() {
        //     $scope.opened = true;
        // }

        //$('button[name="currency-picker"]').iconpicker('setIcon', 'fa-inr');

        //$('button.currencypicker').iconpicker('setIcon', modal.entity.currency === '' ? 'fa-inr' : modal.entity.currency);

        $scope.close = function() {
            //$uibModalInstance.dismiss('');
            var entity = $scope.entity;
            $uibModalInstance.dismiss("user closed the dialog");
        }

        //$scope.currency = $scope.entity || $scope.entity.currency === '' || $scope.entity.currency === 'fa-'
        //  ? 'fa-inr'
        //  : $scope.entity.currency;

        //$scope.getCurrencyIcon = function() {
        //  return $scope.entity.currency === '' || $scope.entity.currency === 'fa-' ? 'fa-inr' : $scope.entity.currency;
        //}

        $scope.save = function() {
                // var ctrl = $('input[name="currency-picker"]');
                // if (ctrl) {
                //     $scope.entity.currency = $('input[name="currency-picker"]').val();
                // }
                $uibModalInstance.close({
                    entity: $scope.entity,
                    entityName: modal.entityName
                });
            }

            //$('[name="currency-picker"]').on('click', function (e) {
            //  $scope.entity.currency = '';
            //  console.log(e);
            //});

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
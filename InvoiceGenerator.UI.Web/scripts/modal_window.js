var module = angular.module('main_page');

module.controller('mController', ['$scope', '$uibModalInstance', 'modal', function($scope, $uibModalInstance, modal) {
    //$scope.modal = modal;

    $scope.percent = /^(100(?:\.0{1,2})?|0{0,2}?\.[\d]{1,2}|[\d]{1,2}(?:\.[\d]{1,2})?)$/;
    $scope.text = /^[a-zA-Z0-9\s.\-,@]+$/;
    $scope.integer = /^\d{1,6}$/;
    $scope.decimal = /^\d*(\.\d+)?$/;
    $scope.email = /^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+(?:[A-Z]{2}|com|org|net|gov|mil|biz|info|mobi|name|aero|jobs|museum|in|co\.in|edu)\b$/;
    $scope.phone = /^(\+\d{1,3} )?\d{2,4}[\- ]?\d{3,4}[\- ]?\d{4}$/;
    $scope.zip = /^\d{5,6}([\-]?\d{4})?$/;

    $scope.entity = modal.entity;

    $scope.backup = {};
    angular.copy($scope.entity, $scope.backup);

    $scope.opened = false;
    $scope.data = modal.scope.data;

    $scope.tabId = 1;

    $scope.getActive = function(item) {
        return item === parseInt($scope.tabId);
    }

    $scope.dateOptions = {
        //dateDisabled: disabled,
        formatYear: 'yyyy',
        maxDate: new Date(2020, 5, 22),
        minDate: new Date(1900, 1, 1),
        startingDay: 1
    };

    $scope.open = function() {
        $scope.opened = true;
    }

    $scope.getBusyStatus = function() {
        if ($scope.entity.logo && $scope.entity.logo.length !== 0) {
            return false;
        }

        return true;

        //return ($scope.entity.logo && $scope.entity.logo.length == 0);
    }

    $scope.close = function() {
        //var entity = $scope.entity;
        angular.copy($scope.backup, $scope.entity);
        $uibModalInstance.dismiss("user closed the dialog");
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

    $scope.save = function() {
        var ctrl = $('input[name="currency-picker"]');
        if (ctrl && ctrl.length > 0) {
            $scope.entity.currency = $('input[name="currency-picker"]').val();
        }

        //modal.scope.isBusy = true;

        $uibModalInstance.close({
            entity: $scope.entity,
            //entityName: modal.entityName
        });
    }

    $scope.getLogo = function() {
        if ($scope.entity.logo && $scope.entity.logo !== '') {
            return 'fa fa-check text-success';
        } else {
            return 'fa fa-times text-danger';
        }
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

        //console.log(ctrl.$name);

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
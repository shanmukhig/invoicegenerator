var module = angular.module('main_page', ['ngRoute', 'ui.bootstrap', 'ngFileUpload']);

module.config(function($routeProvider) {
    $routeProvider.when("/", {
        controller: "vController",
        templateUrl: "templates/main_page.html"
    });

    $routeProvider.otherwise({
        redirectTo: "/"
    });
});

module.controller('vController', ['$scope', '$http', 'dataService', '$uibModal', function($scope, $http, dataService, $uibModal) {
    $scope.data = dataService;
    //$scope.sortType = 'name';
    $scope.sortReverse = false;
    $scope.searchItem = '';
    $scope.tab = dataService.tabs[3];
    //$scope.activeList = [];
    //$scope.isBusy = true;

    $scope.getActive = function(item) {
        return item === parseInt($scope.tab.id);
    }

    $scope.getItems = function(tab) {
        $scope.isBusy = true;
        $scope.tab = tab;
        dataService.getEntity($scope.tab.id, false)
            .then(function(data) {
                $scope.activeList = data;
            }, function() {})
            .finally(function() {
                $scope.isBusy = false;
            });
    }

    $scope.getItems($scope.tab);

    $scope.addressTrim = function(address, length) {

        if (address == undefined) {
            return '';
        }

        var arr = [];
        for (var atr in address) {
            if (address.hasOwnProperty(atr) && address[atr] && address[atr].length > 0 && atr !== 'phone') {
                arr.push(address[atr]);
            }
        }

        var result = arr.join();
        result += (arr.length > 0 ? '. phone:' + address.phone : address.phone);

        if (result.length > length) {
            return result.substring(0, 20) + '...';
        }
        return result;
    }

    //modal popup

    $scope.createOrEdit = function(id) {

        var entity = {};

        if (id) {
            $.each($scope.activeList, function(i, item) {
                if (item.id === id) {
                    entity = item;
                }
            });
        }

        $scope.modalInstance = $uibModal.open({
            templateUrl: $scope.tab.edit_template,
            controller: $scope.tab.uri === 'invoice' ? $scope.tab.uri + 'Controller' : 'mController',
            size: 'xlg',
            animation: true,
            keyboard: false,
            backdrop: false,
            bindToController: true,
            resolve: {
                modal: function() {
                    return {
                        entity: entity,
                        //entityName: $scope.tab.uri,
                        scope: $scope
                    };
                }
            }
        });

        $scope.modalInstance.rendered.then(function() {
            var ctrl = $('i.fa.fa-');
            if (ctrl && ctrl.length > 0) {
                ctrl.removeClass('fa-').addClass(entity.currency && entity.currency === '' ? 'fa-inr' : entity.currency);
            }
        });

        $scope.modalInstance.result.then(onSaveClicked, onCloseClicked);
    }

    var onSaveClicked = function(result) {
        $scope.isBusy = true;
        dataService.saveEntity(result.entity, $scope.tab.uri).then(onSaveSuccess, onSaveFailure);
    };

    var onSaveSuccess = function(result) {
        dataService.getEntity($scope.tab.id, true).then(function(data) {
                $scope.activeList = data;
            }, function() {})
            .finally(function() {
                $scope.isBusy = false;
            });
    }

    var onSaveFailure = function(reason) {
        //console.log;
    }

    var onCloseClicked = function(reason) {
            console.log(reason);
            entity = {};
        }
        //sorting

    $scope.changeSort = function(name) {
        $scope.sortType = name;
        $scope.sortReverse = !$scope.sortReverse;
    }

    $scope.sortIcon = function() {
        return $scope.sortReverse ? 'fa-chevron-up' : 'fa-chevron-down';
    }

    //select, unselect, select all functionality

    $scope.isAllSelected = false;

    $scope.toggleAll = function() {
        $scope.isAllSelected = !$scope.isAllSelected;
        angular.forEach($scope.activeList, function(itm) {
            itm.selected = $scope.isAllSelected;
        });

    }

    $scope.optionToggled = function(i) {
        i.selected = !i.selected;
        $scope.isAllSelected = $scope.activeList.every(function(itm) {
            return itm.selected;
        });
    }
}]);
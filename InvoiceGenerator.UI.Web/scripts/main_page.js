var module = angular.module('main_page', ['ngRoute', 'ui.bootstrap', 'ngFileUpload']);

module.config(function($routeProvider) {
    // $routeProvider.when("/", {
    //     controller: "vController",
    //     templateUrl: "templates/dashboard.html"
    // });

    $routeProvider.when("/", {
        controller: "vController",
        templateUrl: "templates/main_page.html"
    });

    $routeProvider.otherwise({
        redirectTo: "/"
    });
});

module.controller('vController', ['$scope', '$http', 'dataService', '$uibModal', '$filter', '$sce', function($scope, $http, dataService, $uibModal, $filter, $sce) {
    $scope.data = dataService;
    $scope.sortReverse = false;
    $scope.searchItem = '';
    $scope.tab = dataService.tabs[4]; //set this element to chagne default tab. index 0 will show first tab.

    $scope.getActive = function(item) {
        return item === parseInt($scope.tab.id);
    }

    $scope.getItems = function(tab) {
        $scope.isBusy = true;
        $scope.tab = tab;
        dataService.getEntity($scope.tab.id, false)
            .then(function(data) {
                $scope.activeList = data;
            }, function() {
                $scope.activeList = [];
            })
            .finally(function() {
                $scope.isBusy = false;
                if ($scope.tab.id == 5) {
                    $scope.animateProgress();
                }
            });
    }

    $scope.getItems($scope.tab);

    $scope.animateProgress = function() {
        $('.progress-bar').each(function() {
            var bar_value = $(this).attr('aria-valuenow') + '%';
            $(this).animate({
                width: bar_value
            }, {
                duration: 1000
            });
        });
    };

    $scope.reloadEntities = function() {
        $scope.isBusy = true;
        dataService.getEntity($scope.tab.id, true).then(function(data) {
                $scope.activeList = data;
            }, function() {})
            .finally(function() {
                $scope.isBusy = false;
            });
    }

    //modal popup

    $scope.createOrEdit = function(id) {

        var entity = {};

        if (id) {
            entity = $filter('filter')($scope.activeList, {
                id: id
            })[0];
            // $.each($scope.activeList, function(i, item) {
            //     if (item.id === id) {
            //         entity = item;
            //     }
            // });
        }

        $scope.modalInstance = $uibModal.open({
            templateUrl: $scope.tab.edit_template,
            controller: $scope.tab.uri + 'Controller',
            size: 'xlg',
            animation: true,
            keyboard: false,
            backdrop: false,
            bindToController: true,
            resolve: {
                modal: function() {
                    return {
                        entity: entity,
                        scope: $scope //do we need scope in the popup?
                    };
                }
            }
        });

        $scope.modalInstance.rendered.then(function() {
            var ctrl = $('i.fa.fa-inr');
            if (ctrl && ctrl.length > 0 && entity.currency) {
                ctrl.removeClass('fa-inr').addClass(entity.currency);
            }
        });

        $scope.modalInstance.result.then(onSaveClicked, onCloseClicked);
    }

    var onSaveClicked = function(result) {
        $scope.isBusy = true;
        dataService.saveEntity(result.entity, $scope.tab.uri).then(onSaveSuccess(result), onSaveFailure);
    };

    var onSaveSuccess = function(result) {
        if (result.entity.id) {
            console.log('edit existing item, no need to reload');
            $scope.isBusy = false;
            return;
        }

        console.log("created new item, reload complete list");

        dataService.getEntity($scope.tab.id, true).then(function(data) {
                $scope.activeList = data;
            }, function() {})
            .finally(function() {
                $scope.isBusy = false;
            });
    }

    var onSaveFailure = function(reason) {
        $scope.isBusy = false;
    }

    var onCloseClicked = function(reason) {
        console.log(reason);
        entity = {};
    }

    $scope.getAddressHtml = function(element) {
        if (element.addressHtml) {
            return element.addressHtml;
        }

        var str = '<ul class="nav nav-stacked" style="min-width:250px;">';

        str += '<li><a>' + element.address.address1 + '</a></li>';
        str += '<li><a>' + element.address.address2 + '</a></li>';
        str += '<li><a>' + element.address.city + ', ' + element.address.state + ', ' + element.address.zip + '</a></li>'
        str += '<li><a>' + element.address.countryCode + '</a></li>';

        element.addressHtml = $sce.trustAsHtml(str);
        return element.addressHtml;

    }

    $scope.getCommHtml = function(element) {
        if (element.commHtml) {
            return element.commHtml;
        }

        var str = '<ul class="nav nav-stacked" style="min-width:250px;">';
        str += '<li><a><i class="fa fa-envelope"></i> ' + element.address.email + '</a></li>';
        str += '<li><a><i class="fa fa-phone-square"></i> ' + element.address.phone + '</a></li>';
        if (element.contactName) {
            str += '<li><a><i class="fa fa-user"></i> ' + element.contactName + '</a></li>';
        }

        str += '</ul>';

        element.commHtml = $sce.trustAsHtml(str);
        return element.commHtml;
    }

    $scope.getSwiftHtml = function(element) {
        if (element.swiftHtml) {
            return element.swiftHtml;
        }

        var str = '<ul class="nav nav-stacked" style="min-width:250px;">'
        str += '<li><a>' + element.swift.beneficiary + '</a></li>';
        str += '<li><a>' + element.swift.bankName + '</a></li>';
        str += '<li><a>' + element.swift.accountNo + '</a></li>';
        str += '<li><a>' + element.swift.code + '</a></li>';
        str += '<li><a>' + element.swift.branch + '</a></li>';
        str += '</ul>';

        element.swiftHtml = $sce.trustAsHtml(str);
        return element.swiftHtml;
    }

    $scope.getIfscHtml = function(element) {
        if (element.ifscHtml) {
            return element.ifscHtml;
        }

        var str = '<ul class="nav nav-stacked" style="min-width:250px;">'
        str += '<li><a>' + element.ifsc.beneficiary + '</a></li>';
        str += '<li><a>' + element.ifsc.bankName + '</a></li>';
        str += '<li><a>' + element.ifsc.accountNo + '</a></li>';
        str += '<li><a>' + element.ifsc.code + '</a></li>';
        str += '<li><a>' + element.ifsc.branch + '</a></li>';
        str += '</ul>';

        element.ifscHtml = $sce.trustAsHtml(str);
        return element.ifscHtml;
    }

    $scope.getChequeHtml = function(element) {
        if (element.chequeHtml) {
            return element.chequeHtml;
        }

        var str = '<ul class="nav nav-stacked" style="min-width:250px;"><li><a>' + element.cheque.bankName + '</a></li></ul>';

        element.chequeHtml = $sce.trustAsHtml(str);
        return element.chequeHtml;
    }

    $scope.getCompanyTaxesHtml = function(element) {

        if (element.companyTaxesHtml) {
            return element.companyTaxesHtml;
        }

        var str = '<ul class="nav nav-stacked" style="min-width:250px;">';
        str += '<li><a>' + element.tax.serviceTaxNo + '<span class="badge pull-right">Service tax no</span></a></li>';
        str += '<li><a>' + element.tax.tin + '<span class="badge pull-right">TIN</span></a></li>';
        str += '<li><a>' + element.tax.pan + '<span class="badge pull-right">PAN</span></a></li>';
        str += '<li><a>' + element.tax.cin + '<span class="badge pull-right">CIN</span></a></li>';
        str += '</ul>';

        element.companyTaxesHtml = $sce.trustAsHtml(str);
        return element.companyTaxesHtml;
    }

    $scope.getTaxesHtml = function(element) {

        if (element.taxHtml) {
            return element.taxHtml;
        }

        var str = '<ul class="nav nav-stacked" style="min-width:250px;">';
        $.each(element.taxes, function(i, item) {
            str += '<li><a>' + item.name + '<span class="pull-right badge">' + item.percent + '%</span></a></li>';
        });
        str += '</ul>';

        element.taxHtml = $sce.trustAsHtml(str);
        return element.taxHtml;
    }

    $scope.getSupportHtml = function(element) {
        if (element.supportHtml) {
            return element.supportHtml;
        }

        var str = '<ul class="nav nav-stacked" style="min-width:250px;">';
        str += '<li><a><i class="fa fa-envelope"></i> ' + element.support.email + '</a></li>';
        str += '<li><a><i class="fa fa-phone-square"></i> ' + element.support.phone + '</a></li>';

        str += '</ul>';

        element.supportHtml = $sce.trustAsHtml(str);
        return element.supportHtml;
    }

}]);
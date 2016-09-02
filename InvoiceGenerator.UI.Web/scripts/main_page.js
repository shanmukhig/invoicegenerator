;(function() {
  'use strict';

  var module = angular.module('main_page', ['ngRoute', 'ui.bootstrap', 'ngFileUpload']);

  module.config(function($routeProvider) {
    // $routeProvider.when("/", {
    //     controller: "vController",
    //     templateUrl: "templates/dashboard.html"
    // });

    $routeProvider.when("/",
    {
      controller: "vController",
      templateUrl: "templates/common/main_page.html"
    });

    $routeProvider.otherwise({
      redirectTo: "/"
    });
  });

  module.controller('vController',
  ['$scope', '$http', 'dataService', '$uibModal', '$filter', '$sce',
    function($scope, $http, dataService, $uibModal, $filter, $sce) {
      $scope.data = dataService;
      $scope.sortReverse = false;
      $scope.searchItem = '';
      $scope.tab = dataService.tabs[0]; //set this element to chagne default tab. index 0 will show first tab.

      $scope.getActive = function(item) {
        return item === parseInt($scope.tab.id);
      }

      $scope.getItems = function(tab) {
        $scope.isBusy = true;
        $scope.tab = tab;
        dataService.getEntity($scope.tab.id, false)
          .then(function(data) {
              $scope.activeList = data;
            },
            function() {
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

      $scope.reloadEntities = function() {
        $scope.isBusy = true;
        dataService.getEntity($scope.tab.id, true)
          .then(function(data) {
              $scope.activeList = data;
            },
            function() {})
          .finally(function() {
            $scope.isBusy = false;
          });
      }

      //modal popup

      $scope.createOrEdit = function(id) {

        var entity = {};

        if (id) {
          entity = $filter('filter')($scope.activeList,
          {
            id: id
          })[0];
        }

        var editable = {};

        angular.copy(entity, editable);

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
                entity: editable,
                entityName: $scope.tab.uri
              };
            }
          }
        });

        var onSaveClicked = function () {
          $scope.isBusy = true;
          dataService.getEntity($scope.tab.id, true)
            .then(function (data) {
              $scope.activeList = data;
            },
              function (reason) { console.log(reason) })
            .finally(function () {
              $scope.isBusy = false;
            });
        };

        var onCloseClicked = function (reason) {
          console.log(reason);
        };

        $scope.modalInstance.rendered.then(function() {
          var ctrl = $('i.fa.fa-inr');

          if (ctrl.length > 0 && entity.currency) {
            ctrl.removeClass('fa-inr').addClass(entity.currency);
          }
        });
        $scope.modalInstance.result.then(onSaveClicked, onCloseClicked);
      }

      $scope.getAddressHtml = function(element) {
        if (element.addressHtml) {
          return element.addressHtml;
        }

        var str = '<ul class="nav nav-stacked" style="min-width:250px;">';

        str += '<li><a>' + element.address.address1 + '</a></li>';
        str += '<li><a>' + element.address.address2 + '</a></li>';
        str += '<li><a>' + element.address.city + ', ' + element.address.state + ', ' + element.address.zip + '</a></li>';
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

        var str = '<ul class="nav nav-stacked" style="min-width:250px;"><li><a>' +
          element.cheque.bankName +
          '</a></li></ul>';

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
        $.each(element.taxes,
          function(i, item) {
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

      $scope.getProductHtml = function(e) {
        if (e.productHtml) {
          return e.productHtml;
        }

        if (!e.bills || e.bills.length === 0) {
          e.productHtml = $sce.trustAsHtml('<ul class="nav nav-stacked" style="min-width:250px;"><li><a>No products added.</a></li></ul>');
          return e.productHtml;
        }

        var str = '<ul class="nav nav-stacked" style="min-width:250px;">';
        $.each(e.bills,
          function(i, item) {
            str += '<li><a>' + (item.productName) + '</a></li>';
          });
        str += '</ul>';

        e.productHtml = $sce.trustAsHtml(str);
        return e.productHtml;
      }

      $scope.getInvoiceHtml = function(e) {
        if (e.invoiceHtml) {
          return e.invoiceHtml;
        }

        var str = '<ul class="nav nav-stacked" style="min-width:250px;">';
        str += '<li><a>' + $filter('date')(e.billDate, dataService.shortDateFormat) + ' <span class="badge pull-right">Invoice Date</span></a></li>';
        str += '<li><a>' + $filter('date')(e.dueDate, dataService.shortDateFormat) + ' <span class="badge pull-right">Due Date</span></a></li>';
        str += '<li><a>' + $filter('date')(e.startDate, dataService.shortDateFormat) + ' <span class="badge pull-right">Start Date</span></a></li>';
        str += '<li><a>' + $filter('date')(e.endDate, dataService.shortDateFormat) + ' <span class="badge pull-right">End Date</span></a></li>';

        str += '</ul>';

        e.invoiceHtml = $sce.trustAsHtml(str);
        return e.invoiceHtml;
      }

      $scope.downloadPdf = function (id, invoiceNo) {
        $scope.isBusy = true;
        dataService.getEntity(6, id)
          .then(function(data) {
              var file = new Blob([data], { type: 'application/octet-stream' });
              var fileUrl = URL.createObjectURL(file);
              var a = document.createElement('a');
              a.href = fileUrl;
              a.download = invoiceNo + '.pdf';
              a.target = '_blank';
              a.click();
              dataService.notifySuccess({
                title: 'Generate Pdf',
                message: 'Pdf generated and download successfully'
              });
            },
            function(reason) {
              console.log(reason);
              dataService.notifyError({
                title: 'Generate pdf',
                message: 'Error occured while generating pdf, please try again later'
              });
            })
          .finally(function() {
            $scope.isBusy = false;
          });
      }
    }
  ]);
}());
module.config(function ($routeProvider) {
  $routeProvider.when('/payments/create',
  {
    controller: 'paymentsController',
    templateUrl: 'templates/payments_create.html'
  });
  $routeProvider.when("/payments/update/:id",
  {
    controller: 'paymentsController',
    templateUrl: 'templates/payments_create.html'
  });
  $routeProvider.otherwise({ redirectTo: "/" });
});

module.controller('paymentsController', ['$scope', '$http', '$dataService', '$routeParams', '$window', function ($scope, $http, dataService, $routeParams, $window) {
  if ($routeParams.id === undefined || $routeParams.id === '') {
    $scope.payment = [];
    $scope.isNew = true;
  } else {
    $scope.payment = dataService.getItemById($routeParams.id);
  }
}]);
var module = angular.module('main_page');

module.factory('dataService', function($http, $q) {

    var baseUri = '/invoicegenerator.api/api/';

    function getEntities(source, uriPart) {
        var deferred = $q.defer();

        $http
            .get(baseUri + uriPart)
            .then(
                function(result) {
                    if (result.data.length > 0) {
                        if (source) {
                            angular.copy(result.data, source);
                        }
                        deferred.resolve(result.data);
                    } else {
                        source = [];
                        deferred.resolve(source);
                    }
                },
                function(error) {
                    deferred.reject(error.data.message);
                });
        return deferred.promise;
    }

    var tabs = [{
        'id': 1,
        'name': 'Companies',
        'fontIcon': 'university',
        'view_template': 'templates/companies.html',
        'edit_template': 'templates/create_company.html',
        'sortColumn': 'name',
        'uri': 'company'
    }, {
        'id': 2,
        'name': 'Customers',
        'fontIcon': 'users',
        'view_template': 'templates/customers.html',
        'edit_template': 'templates/create_customer.html',
        'sortColumn': 'name',
        'uri': 'customer'
    }, {
        'id': 3,
        'name': 'Products',
        'fontIcon': 'tags',
        'view_template': 'templates/products.html',
        'edit_template': 'templates/create_product.html',
        'sortColumn': 'name',
        'uri': 'product'
    }, {
        'id': 4,
        'name': 'Invoices',
        'fontIcon': 'file-text',
        'view_template': 'templates/invoices.html',
        'edit_template': 'templates/create_invoice.html',
        'sortColumn': 'invoiceDate',
        'uri': 'invoice'
    }, {
        'id': 5,
        'name': 'Payments',
        'fontIcon': 'money',
        'view_template': 'templates/payments.html',
        'edit_template': 'templates/create_payment.html',
        'sortColumn': 'paymentDate',
        'uri': 'payment'
    }];

    var frequencies = [{
        id: 0,
        name: 'Other'
    }, {
        id: 1,
        name: 'Monthly'
    }, {
        id: 3,
        name: 'Quarterly'
    }, {
        id: 6,
        name: 'Halfyearly'
    }, {
        id: 12,
        name: 'Yearly'
    }, ];

    var companies = [];
    var companiesLoaded = false;

    var getCompanies = function(reload) {
        if (!companiesLoaded || reload) {
            companiesLoaded = true;
            return getEntities(companies, 'company');
        }
        return $q.resolve(companies);
    }

    var customers = [];
    var customersLoaded = false;

    var getCustomers = function(reload) {
        if (!customersLoaded || reload) {
            customersLoaded = true;
            return getEntities(customers, 'customer');
        }
        return $q.resolve(customers);
    }

    var products = [];
    var productsLoaded = false;

    var getProducts = function(reload) {
        if (!productsLoaded || reload) {
            productsLoaded = true;
            return getEntities(products, 'product');
        }
        return $q.resolve(products);
    }

    var invoices = [];
    var invoicesLoaded = false;
    var getInvoices = function(reload) {
        if (!invoicesLoaded || reload) {
            invoicesLoaded = true;
            return getEntities(invoices, 'invoice');
        }
        return $q.resolve(invoices);
    }

    var payments = [];
    var paymentsLoaded = false;

    var getPayments = function(reload) {
        if (!paymentsLoaded || reload) {
            paymentsLoaded = true;
            return getEntities(payments, 'payment');
        }
        return $q.resolve(payments);
    }

    var saveEntity = function(entity, entityName, upload) {
        var deferred = $q.defer();

        $http({
                url: entity.id ? baseUri + entityName + '/' + entity.id : baseUri + entityName,
                method: entity.id ? 'PUT' : 'POST',
                data: entity
            })
            .success(function(data) {
                console.log(data);
                deferred.resolve(data);
            })
            .error(function(data) {
                console.log(data);
                deferred.reject(data);
            });

        return deferred.promise;
    }

    var getEntity = function(entityName, reload) {
        switch (entityName) {
            case 1:
                return getCompanies(reload);
            case 2:
                return getCustomers(reload);
            case 3:
                return getProducts(reload);
            case 4:
                return getInvoices(reload);
            case 5:
                return getPayments(reload);
        }
    }

    return {
        tabs: tabs,
        getEntity: getEntity,
        saveEntity: saveEntity,
        frequencies: frequencies
    };
});
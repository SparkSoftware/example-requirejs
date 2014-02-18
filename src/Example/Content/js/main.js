require.config({
    paths: {
        'jquery': ['//ajax.aspnetcdn.com/ajax/jQuery/jquery-1.11.0.min', 'vendor/jquery/jquery'],
        'knockout': ['//ajax.aspnetcdn.com/ajax/knockout/knockout-3.0.0', 'vendor/knockout/knockout'],
        'bootstrap': ['//ajax.aspnetcdn.com/ajax/bootstrap/3.1.0/bootstrap.min', 'vendor/bootstrap/bootstrap']
    },
    shim: {
        'bootstrap': ['jquery']
    },
    map: {
        '*': {
            'pkg/jquery': 'lib/jquery/package',
            'pkg/knockout': 'lib/knockout/package'
        }
    }
});

define(['config', 'pkg/jquery', 'pkg/knockout'], function() { });
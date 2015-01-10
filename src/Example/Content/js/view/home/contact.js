define(['pkg/jquery', './shared/_address', './shared/_email'], function ($, address, email) {
    $.log('Loaded: view/home/contact');

    return {
        address: address,
        email: email
    };
});

define(['pkg/jquery', './address', './email'], function ($, address, email) {
    $.log('Loaded: view/home/contact');

    return {
        address: address,
        email: email
    };
});

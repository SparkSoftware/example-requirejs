define(['jquery', 'pkg/knockout'], function ($, ko) {
    return {
        street1: ko.observable('One Microsoft Way'),
        street2: ko.observable('Redmond, WA 98052-6399'),
        phone: ko.observable('425.555.0100')
    };
});

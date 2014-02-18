require(['jquery'], function ($) {
    var loaded = false;

    function getDependencies(node) {
        var model = node.getAttribute('data-model'),
            require = node.getAttribute('data-require'),
            dependencies = require && require.length > 0 ? require.split(',') : [], distinct = {}, result = [], dependency, i;

        // Check if we are binding a view model, add the model as well as knockout to the dependency list.
        if (model) {
            result.push(model);
            distinct[model] = true;

            result.push('knockout');
            distinct['knockout'] = true;
        }

        // Remove duplicates and whitespace.
        for (i = dependencies.length - 1; i >= 0; i -= 1) {
            dependency = dependencies[i].replace(/^\s+|\s+$/g, '');

            if (dependency && !distinct.hasOwnProperty(dependency)) {
                distinct[dependency] = true;
                result.push(dependency);
            }
        }

        return result;
    }

    function loadDependencies(node, dependencies) {
        // Ensure the list attribute value is not empty.
        if (dependencies.length > 0) {
            require(dependencies, function (viewModel, ko) {
                // Apply the knockout bindings using the provided view model if not null.
                if (ko && ko.applyBindings && viewModel) {
                    ko.applyBindings(viewModel, node);
                }

                bindChildren(node);
            });
        }
    }

    function bindChildren(node) {
        var childNode;

        // Traverse the set of child nodes if exists.
        if (node && node.hasChildNodes && node.hasChildNodes()) {
            childNode = node.firstChild;

            while (childNode) {
                bind(childNode);

                childNode = childNode.nextSibling;
            }
        }
    }

    function bind(node) {
        // Determine if the node has a `data-require` attribute and load the specified required module.
        if (node && node.hasAttribute && (node.hasAttribute('data-model') || node.hasAttribute('data-require'))) {
            loadDependencies(node, getDependencies(node));
        } else {
            bindChildren(node);
        }
    }

    // Ensure the DOM is ready before scanning for binding attributes.
    $(function () {
        if (!loaded) {
            loaded = true;
            bind(document.body);
        }
    });
});
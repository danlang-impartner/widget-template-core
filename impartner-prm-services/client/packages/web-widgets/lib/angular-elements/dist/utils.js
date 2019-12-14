"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
/**
 * @license
 * Copyright Google Inc. All Rights Reserved.
 *
 * Use of this source code is governed by an MIT-style license that can be
 * found in the LICENSE file at https://angular.io/license
 */
var core_1 = require("@angular/core");
var elProto = Element.prototype;
var matches = elProto.matches ||
    elProto.matchesSelector ||
    elProto.mozMatchesSelector ||
    elProto.msMatchesSelector ||
    elProto.oMatchesSelector ||
    elProto.webkitMatchesSelector;
/**
 * Provide methods for scheduling the execution of a callback.
 */
exports.scheduler = {
    /**
     * Schedule a callback to be called after some delay.
     *
     * Returns a function that when executed will cancel the scheduled function.
     */
    schedule: function (taskFn, delay) {
        var id = setTimeout(taskFn, delay);
        return function () { return clearTimeout(id); };
    },
    /**
     * Schedule a callback to be called before the next render.
     * (If `window.requestAnimationFrame()` is not available, use `scheduler.schedule()` instead.)
     *
     * Returns a function that when executed will cancel the scheduled function.
     */
    scheduleBeforeRender: function (taskFn) {
        // TODO(gkalpak): Implement a better way of accessing `requestAnimationFrame()`
        //                (e.g. accounting for vendor prefix, SSR-compatibility, etc).
        if (typeof window === 'undefined') {
            // For SSR just schedule immediately.
            return exports.scheduler.schedule(taskFn, 0);
        }
        if (typeof window.requestAnimationFrame === 'undefined') {
            var frameMs = 16;
            return exports.scheduler.schedule(taskFn, frameMs);
        }
        var id = window.requestAnimationFrame(taskFn);
        return function () { return window.cancelAnimationFrame(id); };
    }
};
/**
 * Convert a camelCased string to kebab-cased.
 */
function camelToDashCase(input) {
    return input.replace(/[A-Z]/g, function (char) { return "-" + char.toLowerCase(); });
}
exports.camelToDashCase = camelToDashCase;
/**
 * Create a `CustomEvent` (even on browsers where `CustomEvent` is not a constructor).
 */
function createCustomEvent(doc, name, detail) {
    var bubbles = false;
    var cancelable = false;
    // On IE9-11, `CustomEvent` is not a constructor.
    if (typeof CustomEvent !== 'function') {
        var event_1 = doc.createEvent('CustomEvent');
        event_1.initCustomEvent(name, bubbles, cancelable, detail);
        return event_1;
    }
    return new CustomEvent(name, { bubbles: bubbles, cancelable: cancelable, detail: detail });
}
exports.createCustomEvent = createCustomEvent;
/**
 * Check whether the input is an `Element`.
 */
function isElement(node) {
    return !!node && node.nodeType === Node.ELEMENT_NODE;
}
exports.isElement = isElement;
/**
 * Check whether the input is a function.
 */
function isFunction(value) {
    return typeof value === 'function';
}
exports.isFunction = isFunction;
/**
 * Convert a kebab-cased string to camelCased.
 */
function kebabToCamelCase(input) {
    return input.replace(/-([a-z\d])/g, function (_, char) { return char.toUpperCase(); });
}
exports.kebabToCamelCase = kebabToCamelCase;
/**
 * Check whether an `Element` matches a CSS selector.
 */
function matchesSelector(element, selector) {
    return matches.call(element, selector);
}
exports.matchesSelector = matchesSelector;
/**
 * Test two values for strict equality, accounting for the fact that `NaN !== NaN`.
 */
function strictEquals(value1, value2) {
    return value1 === value2 || (value1 !== value1 && value2 !== value2);
}
exports.strictEquals = strictEquals;
/** Gets a map of default set of attributes to observe and the properties they affect. */
function getDefaultAttributeToPropertyInputs(inputs) {
    var attributeToPropertyInputs = {};
    inputs.forEach(function (_a) {
        var propName = _a.propName, templateName = _a.templateName;
        attributeToPropertyInputs[camelToDashCase(templateName)] = propName;
    });
    return attributeToPropertyInputs;
}
exports.getDefaultAttributeToPropertyInputs = getDefaultAttributeToPropertyInputs;
/**
 * Gets a component's set of inputs. Uses the injector to get the component factory where the inputs
 * are defined.
 */
function getComponentInputs(component, injector) {
    var componentFactoryResolver = injector.get(core_1.ComponentFactoryResolver);
    var componentFactory = componentFactoryResolver.resolveComponentFactory(component);
    return componentFactory.inputs;
}
exports.getComponentInputs = getComponentInputs;

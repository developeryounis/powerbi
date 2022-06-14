"use strict";
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
Object.defineProperty(exports, "__esModule", { value: true });
exports.successElement = exports.successClass = exports.reportListUrl = exports.reportUrl = exports.position = exports.hidden = exports.errorElement = exports.errorClass = void 0;
// Success Image element
var successElement = document.createElement('img');
exports.successElement = successElement;
successElement.className = 'status-img';
successElement.src = '/assets/success.svg';
// Error Image element
var errorElement = document.createElement('img');
exports.errorElement = errorElement;
errorElement.className = 'status-img';
errorElement.src = '/assets/error.svg';
// Endpoint to get report config
//const reportUrl = 'https://aka.ms/CaptureViewsReportEmbedConfig';
var reportUrl = 'api/embedreport';
exports.reportUrl = reportUrl;
var reportListUrl = 'api/embedreport/getreportlist';
exports.reportListUrl = reportListUrl;
var errorClass = 'error';
exports.errorClass = errorClass;
var successClass = 'success';
exports.successClass = successClass;
// To show / hide the report container
var hidden = 'hidden';
exports.hidden = hidden;
// To position the display message
var position = 'position';
exports.position = position;
//# sourceMappingURL=constants.js.map
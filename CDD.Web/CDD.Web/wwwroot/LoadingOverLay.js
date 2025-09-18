"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.LoadingOverLay = void 0;
var LoadingOverLay = /** @class */ (function () {
    function LoadingOverLay() {
        this.options = {
            containerID: null,
            backgroundColor: '#eee',
            backgroundOpacity: 0.6,
            disableScroll: false,
            overlayID: 'MyOverLay',
            overlayZIndex: 99998,
            spinnerID: 'MySpinner',
            spinnerSize: 50,
            spinnerZIndex: 99999
        };
        this.spinnerHtml = "<img style=\"width: 80px;\" src=\"/loading-icon-bk.gif\"/><span>\u8B80\u53D6\u4E2D</span>";
    }
    /**
     * 顯示遮罩
     */
    LoadingOverLay.prototype.show = function (options, spinnerHtml) {
        if (options === void 0) { options = null; }
        if (spinnerHtml === void 0) { spinnerHtml = null; }
        this.SetOptions(options);
        this.hide();
        //
        this.SetSpinner(spinnerHtml);
        if (this.options.disableScroll) {
            document.body.style.overflow = 'hidden';
            document.documentElement.style.overflow = 'hidden';
        }
        // 產生遮罩
        this.CreateOverlayElement();
    };
    /**
     * 隱藏遮罩
     */
    LoadingOverLay.prototype.hide = function () {
        var _a, _b;
        // remove disabled scroll style
        if (this.options.disableScroll) {
            document.body.style.overflow = '';
            document.documentElement.style.overflow = '';
        }
        (_a = document.getElementById(this.options.overlayID)) === null || _a === void 0 ? void 0 : _a.remove();
        (_b = document.getElementById(this.options.spinnerID)) === null || _b === void 0 ? void 0 : _b.remove();
    };
    LoadingOverLay.prototype.SetOptions = function (options) {
        if (typeof options !== 'undefined') {
            for (var key in options) {
                this.options[key] = options[key];
            }
        }
    };
    LoadingOverLay.prototype.SetSpinner = function (html) {
        if (typeof html !== 'undefined' && html !== null && html.trim()) {
            this.spinnerHtml = html;
        }
    };
    LoadingOverLay.prototype.CreateOverlayElement = function () {
        var left = '50%';
        // left = `calc(50% + ${this.options.offsetX})`
        var top = '50%';
        //  top = `calc(50% + ${this.options.offsetY})`
        // Generate overlay html element in container.
        if (this.options.containerID &&
            document.body.contains(document.getElementById(this.options.containerID))) {
            var loadingOverlay = "<div id=\"".concat(this.options.overlayID, "\" \n                style=\"display: block !important; position: absolute; top: 0; left: 0; overflow: auto;\n                opacity: ").concat(this.options.backgroundOpacity, "; \n                background: ").concat(this.options.backgroundColor, "; \n                z-index: 50; width: 100%; height: 100%;\">\n            </div>\n            <div id=\"").concat(this.options.spinnerID, "\" \n                style=\"\n                display: flex;\n                -webkit-box-pack: center;\n                -ms-flex-pack: center;\n                justify-content: center;\n                -webkit-box-align: center;\n                -ms-flex-align: center;\n                align-items: center;\n                -webkit-box-orient: vertical;\n                -webkit-box-direction: normal;\n                -ms-flex-direction: column;\n                flex-direction: column;\n                position: relative; \n                z-index: 9999;\">").concat(this.spinnerHtml, "</div>");
            var containerElement = document.getElementById(this.options.containerID);
            if (containerElement) {
                containerElement.style.position = 'relative';
                containerElement.insertAdjacentHTML('beforeend', loadingOverlay);
            }
            return;
        }
        else {
            var loadingOverlay = "<div id=\"".concat(this.options.overlayID, "\" \n            style=\"display: block !important; position: fixed; top: 0; left: 0; overflow: auto; \n            background: ").concat(this.options.backgroundColor, "; \n            opacity: ").concat(this.options.backgroundOpacity, "; \n            z-index: ").concat(this.options.overlayZIndex, "; width: 100%; height: 100%;\"></div>\n            <div id=\"").concat(this.options.spinnerID, "\" \n                style=\"\n                display: flex;\n                width:100%;\n                height:100%;\n                -webkit-box-pack: center;\n                -ms-flex-pack: center;\n                justify-content: center;\n                -webkit-box-align: center;\n                -ms-flex-align: center;\n                align-items: center;\n                -webkit-box-orient: vertical;\n                -webkit-box-direction: normal;\n                -ms-flex-direction: column;\n                flex-direction: column;\n                position: fixed; \n                top: 0; \n                left: 0; \n                z-index: ").concat(this.options.spinnerZIndex, ";\">").concat(this.spinnerHtml, "</div>");
            document.body.insertAdjacentHTML('beforeend', loadingOverlay);
        }
    };
    return LoadingOverLay;
}());
exports.LoadingOverLay = LoadingOverLay;

! function(a, b, c) {
    function d(a) {
        this.title = a.title, this.selector = a.selector, this.$content = a.$content || null, this.$curParent = null
    }

    function e(b, d) {
        var e = b.outerWidth(),
            f = b.outerHeight(),
            g = a(c),
            i = g.height(),
            l = g.width(),
            m = b.find(".jbox-title").outerHeight(),
            n = b.find(".jbox-buttons").outerHeight(),
            o = b.find(".jbox-container"),
            p = h(d);
        if ("auto" == p.width ? (e < p.minWidth && (e = p.minWidth), e > 900 && (e = 900)) : e = p.width, e >= l && (e = l - 50), "auto" == p.height ? (f < p.minHeight && (f = p.minHeight), f >= i && (f = i - 100)) : f = p.height, f >= i && (f = i - 50), o.css("height", f - m - n - parseInt(o.css("paddingTop")) - parseInt(o.css("paddingBottom"))), b.css({
                position: "fixed",
                width: e,
                height: f,
                left: "50%",
                top: "50%",
                marginLeft: -parseInt(e / 2),
                marginTop: -parseInt(f / 2),
                zIndex: 999998
            }), j && 6 >= k) {
            var q = parseInt(g.scrollTop());
            b.css({
                position: "absolute",
                top: i / 2 + q
            })
        }
    }

    function f() {
        a("#jbox-overlay").length < 1 && a("body").append('<div id="jbox-overlay"></div>');
        var b = a("#jbox-overlay"),
            d = a(c),
            e = a("body").height(),
            f = d.height(),
            g = f > e ? f : e;
        if (b.css({
                position: "fixed",
                zIndex: 999997,
                top: "0px",
                left: "0px",
                width: "100%",
                height: g,
                background: "#000",
                opacity: .15
            }), j && 6 >= k) {
            var h = parseInt(d.scrollTop());
            b.css({
                position: "absolute",
                top: h
            })
        }
        b.show()
    }

    function g() {
        a("#jbox-overlay").fadeOut(200)
    }

    function h(b) {
        var c = null,
            d = null,
            e = null;
        return c = a.extend({}, i, b), d = a.extend({}, i.btnOK, b.btnOK), e = a.extend({}, i.btnCancel, b.btnCancel), c.btnOK = d, c.btnCancel = e, c
    }
    var i = {
            title: "提示",
            width: "auto",
            height: "auto",
            minWidth: 400,
            minHeight: 200,
            content: "",
            onOpen: !1,
            onClosed: !1,
            btnOK: {
                text: "确定",
                show: !0,
                extclass: "btn btn-primary",
                onBtnClick: !1
            },
            btnCancel: {
                text: "取消",
                show: !0,
                extclass: "btn",
                onBtnClick: !1
            }
        },
        j = a.browser.msie,
        k = parseInt(a.browser.version);
    d.prototype.create = function(b, c) {
        var d = this.title,
            h = this.selector,
            i = null,
            j = null;
        this.$content = i = this.$content ? this.$content : a(h);
        try {
            this.$curParent = j = i.parent()
        } catch (k) {}
        var l = a('<div class="jbox"></div>'),
            m = a('<div class="jbox-title"></div>'),
            n = a('<div class="jbox-container"></div>'),
            o = a('<div class="jbox-buttons"></div>');
        if (m.append('<div class="jbox-title-txt">' + d + "</div>", '<a href="javascript:;" class="jbox-close" ></a>'), b.btnOK.show) {
            var p = a('<a href="javascript:;" class="jbox-buttons-ok ' + b.btnOK.extclass + '">' + b.btnOK.text + "</a>");
            b.btnOK.onBtnClick && p.click(function() {
                b.btnOK.onBtnClick.call(b.trigger, l)
            }), o.append(p)
        }
        if (b.btnCancel.show) {
            var q = a('<a href="javascript:;" class="jbox-buttons-ok ' + b.btnCancel.extclass + '">' + b.btnCancel.text + "</a>");
            q.click(function() {
                b.btnCancel.onBtnClick ? b.btnCancel.onBtnClick.call(b.trigger, l) : l.find(".jbox-close").triggerHandler("click")
            }), o.append(q)
        }
        n.append(i), b.btnOK.show || b.btnCancel.show ? l.append(m, n, o) : l.append(m, n), a("body").append(l), e(l, b), f(), l.find(".jbox-close").one("click", function() {
            try {
                i.appendTo(j)
            } catch (a) {}
            return b.onClosed && b.onClosed.call(b.trigger), l.remove(), g(), !1
        }), l && c && c.call(b.trigger, l)
    }, a.fn.jBox = function(b) {
        return this.each(function() {
            var c = a(this),
                e = c.data("rule") || "normal",
                f = h(b ? b : {});
            if (f.trigger = this, "box" == e) c.click(function() {
                var a = new d({
                    title: c.attr("title") || f.title,
                    selector: c.attr("href")
                });
                return a.create(f, f.onOpen), !1
            });
            else {
                var g = new d({
                    title: f.title,
                    $content: c
                });
                g.create(f, f.onOpen)
            }
        })
    };
    var l = a.jBox = {};
    l.show = function(a) {
        var b = h(a ? a : {}),
            c = new d({
                title: b.title,
                $content: b.content
            });
        c.create(b, b.onOpen)
    }, l.reposition = function() {
        a(".jbox").each(function() {
            e(a(this))
        })
    }, l.showloading = function() {
        var b = a("#jbox-loading");
        if (!b.length > 0) {
            var b = a('<div id="jbox-loading"><i></i></div>');
            a("body").append(b)
        }
        b.fadeIn(200)
    }, l.hideloading = function() {
        a("#jbox-loading").fadeOut(200)
    }, l.close = function(b) {
        a(b).find(".jbox-close").triggerHandler("click")
    }
}(jQuery, document, window);

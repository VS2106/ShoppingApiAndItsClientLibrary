var ShoppingApiClient = function() {
    var shoppingApiEndPointUrl = "http://localhost:9862", //TODO change to SSL
        accessTockenStorageName = "ShoppingApiAccessToken",
        setHeader = function() {
            return {
                'Authorization': "Bearer "
                    + sessionStorage.getItem(accessTockenStorageName)
            };
        },
        ajaxSuccessCallBack = function(response, textStatus, jqXhr, successCallBack) {
            if ($.isFunction(successCallBack)) {
                successCallBack(response, textStatus, jqXhr);
            }
        },
        ajaxErrorCallBack = function(jqXhr, textStatus, errorThrown, errorCallBack) {
            if ($.isFunction(errorCallBack)) {
                errorCallBack(jqXhr, textStatus, errorThrown);
            }
        },
        login = function(username, password, successCallBack, errorCallBack) {
            $.ajax({
                url: shoppingApiEndPointUrl + "/token",
                method: "POST",
                contentType: "application/json",
                data: {
                    username: username,
                    password: password,
                    grant_type: "password"
                },
                success: function(response, textStatus, jqXhr) {
                    sessionStorage.setItem(accessTockenStorageName, response.access_token);
                    ajaxSuccessCallBack(response, textStatus, jqXhr, successCallBack);
                },
                error: function(jqXhr, textStatus, errorThrown) {
                    ajaxErrorCallBack(jqXhr, textStatus, errorThrown, errorCallBack);
                }
            });
        },
        logout = function() {
            sessionStorage.removeItem(accessTockenStorageName);
        },
        register = function(email, password, confirmPassword, successCallBack, errorCallBack) {
            $.ajax({
                url: shoppingApiEndPointUrl + "/api/account/register",
                method: "POST",
                data: {
                    email: email,
                    password: password,
                    confirmPassword: confirmPassword
                },
                success: function(response, textStatus, jqXhr) {
                    ajaxSuccessCallBack(response, textStatus, jqXhr, successCallBack);
                },
                error: function(jqXhr, textStatus, errorThrown) {
                    ajaxErrorCallBack(jqXhr, textStatus, errorThrown, errorCallBack);
                }
            });
        },
        getProducts = function(successCallBack, errorCallBack) {
            $.ajax({
                url: shoppingApiEndPointUrl + "/api/products",
                method: "Get",
                headers: setHeader(),
                success: function(response, textStatus, jqXhr) {
                    ajaxSuccessCallBack(response, textStatus, jqXhr, successCallBack);
                },
                error: function(jqXhr, textStatus, errorThrown) {
                    ajaxErrorCallBack(jqXhr, textStatus, errorThrown, errorCallBack);
                }
            });
        },
        getShoppingBasket = function(successCallBack, errorCallBack) {
            $.ajax({
                url: shoppingApiEndPointUrl + "/api/ShoppingBasket",
                method: "Get",
                headers: setHeader(),
                success: function(response, textStatus, jqXhr) {
                    ajaxSuccessCallBack(response, textStatus, jqXhr, successCallBack);
                },
                error: function(jqXhr, textStatus, errorThrown) {
                    ajaxErrorCallBack(jqXhr, textStatus, errorThrown, errorCallBack);
                }
            });
        },
        clearOutShoppingBasket = function(successCallBack, errorCallBack) {
            $.ajax({
                url: shoppingApiEndPointUrl + "/api/ShoppingBasket/clearout",
                method: "Put",
                headers: setHeader(),
                success: function(response, textStatus, jqXhr) {
                    ajaxSuccessCallBack(response, textStatus, jqXhr, successCallBack);
                },
                error: function(jqXhr, textStatus, errorThrown) {
                    ajaxErrorCallBack(jqXhr, textStatus, errorThrown, errorCallBack);
                }
            });
        },
        getOrderItem = function(orderItemId, successCallBack, errorCallBack) {
            $.ajax({
                url: shoppingApiEndPointUrl + "/api/OrderItems/" + orderItemId,
                method: "Get",
                headers: setHeader(),
                success: function(response, textStatus, jqXhr) {
                    ajaxSuccessCallBack(response, textStatus, jqXhr, successCallBack);
                },
                error: function(jqXhr, textStatus, errorThrown) {
                    ajaxErrorCallBack(jqXhr, textStatus, errorThrown, errorCallBack);
                }
            });
        },
        addOrderItemToShoppingBasket = function(productId, quantity, successCallBack, errorCallBack) {
            $.ajax({
                url: shoppingApiEndPointUrl + "/api/OrderItems",
                method: "POST",
                headers: setHeader(),
                data: {
                    productId: productId,
                    quantity: quantity
                },
                success: function(response, textStatus, jqXhr) {
                    ajaxSuccessCallBack(response, textStatus, jqXhr, successCallBack);
                },
                error: function(jqXhr, textStatus, errorThrown) {
                    ajaxErrorCallBack(jqXhr, textStatus, errorThrown, errorCallBack);
                }
            });
        },
        changeQuantityOfOrderItem = function(orderItemId, quantity, successCallBack, errorCallBack) {
            $.ajax({
                url: shoppingApiEndPointUrl + "/api/OrderItems/" + orderItemId,
                method: "Put",
                headers: setHeader(),
                data: {
                    quantity: quantity
                },
                success: function(response, textStatus, jqXhr) {
                    ajaxSuccessCallBack(response, textStatus, jqXhr, successCallBack);
                },
                error: function(jqXhr, textStatus, errorThrown) {
                    ajaxErrorCallBack(jqXhr, textStatus, errorThrown, errorCallBack);
                }
            });
        },
        removeOrderItemFromShoppingBasket = function(orderItemId, successCallBack, errorCallBack) {
            $.ajax({
                url: shoppingApiEndPointUrl + "/api/OrderItems/" + orderItemId,
                method: "Delete",
                headers: setHeader(),
                success: function(response, textStatus, jqXhr) {
                    ajaxSuccessCallBack(response, textStatus, jqXhr, successCallBack);
                },
                error: function(jqXhr, textStatus, errorThrown) {
                    ajaxErrorCallBack(jqXhr, textStatus, errorThrown, errorCallBack);
                }
            });
        };


    return {
        Login: login,
        Logout: logout,
        Register: register,
        GetProducts: getProducts,
        GetShoppingBasket: getShoppingBasket,
        ClearOutShoppingBasket: clearOutShoppingBasket,
        GetOrderItem: getOrderItem,
        AddOrderItemToShoppingBasket: addOrderItemToShoppingBasket,
        ChangeQuantityOfOrderItem: changeQuantityOfOrderItem,
        RemoveOrderItemFromShoppingBasket: removeOrderItemFromShoppingBasket
    };
}();
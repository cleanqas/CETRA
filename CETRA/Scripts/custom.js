var helpers =
            {
                buildDropdown: function (result, dropDown, emptyMessage, field, id) {
                    dropDown.html('');
                    dropDown.append('<option value="">' + emptyMessage + '</option>');
                    if (result != '') {
                        $.each(result, function (k, v) {
                            if (field === undefined) {
                                if (id === undefined) {
                                    dropDown.append('<option value="' + v.Id + '">' + v.Name + '</option>');
                                } else {
                                    dropDown.append('<option value="' + v[id] + '">' + v.Name + '</option>');
                                }
                            } else {
                                if (id === undefined) {
                                    dropDown.append('<option value="' + v.Id + '">' + v[field] + '</option>');
                                } else {
                                    dropDown.append('<option value="' + v[id] + '">' + v[field] + '</option>');
                                }
                            }

                        });
                    }
                },

                notify: function (msg) {
                    $('#confirmationModal').modal();
                    $('#notification').text(msg);
                },

                checkEquality: function (f, s) {
                    return f == s;
                },

                getData: function (_url, callback) {
                    $.ajax({
                        type: "POST",
                        url: _url,
                        data: { __RequestVerificationToken: helpers.getAntiForgeryToken() },
                        contentType: 'application/x-www-form-urlencoded',
                        success: function (data) {
                            callback(data);
                        }
                    });
                },

                getAntiForgeryToken: function () {
                    var form = $('#_AjaxAntiForgeryForm');
                    var token = $('input[name="__RequestVerificationToken"]', form).val();
                    console.log(token);
                    return token;
                },

                getBankName: function (bankId, banks) {
                    console.log(banks);
                    console.log(bankId);
                    $.each(banks, function (i, v) {
                        if (v.Id == bankId) {
                            return v.BankName;
                        }
                    });
                }
            };

# LIBJSON_CHECK_CONFIG ([DEFAULT-ACTION])
# ----------------------------------------------------------
#    Job Arranger production committee        Oct-17-2014
#
# libjson.html
#
# Checks for json-c.  DEFAULT-ACTION is the string yes or no to
# specify whether to default to --with-json or --without-json.
# If not supplied, DEFAULT-ACTION is no.
#
# This macro is distributed in the hope that it will be useful,
# but WITHOUT ANY WARRANTY; without even the implied warranty of
# MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.

AC_DEFUN([LIBJSON_CHECK_CONFIG],
[
    AC_ARG_WITH([json],
        AC_HELP_STRING(
            [--with-json@<:@=ARG@:>@],
            [use json-c library @<:@default=yes@:>@, optionally specify the prefix for json-c library]
        ),
        [
            WANT_JSON="yes"
            _json_path="$withval"
        ],
        [WANT_JSON="yes"]
    )

    found_json="no"
    JSON_CFLAGS=""
    JSON_CPPFLAGS=""
    JSON_LDFLAGS=""
    JSON_LIBS=""
    if test "x$WANT_JSON" = "xyes"; then
	AC_MSG_CHECKING(for JSON-C support)
        for _json_path_tmp in $_json_path /usr /usr/local /opt ; do
           if test -f "$_json_path_tmp/include/json-c/json.h" && test -r "$_json_path_tmp/include/json-c/json.h"; then
               if test -f "$_json_path_tmp/lib/libjson-c.a" && test -r "$_json_path_tmp/lib/libjson-c.a"; then
                  _json_cppflags="-I$_json_path_tmp/include/json-c"
                  _json_ldflags="-L$_json_path_tmp/lib"
                  _json_libs="$_json_path_tmp/lib/libjson-c.a"
                  found_json="yes"
                  break;
               fi
           elif test -f "$_json_path_tmp/include/json/json.h" && test -r "$_json_path_tmp/include/json/json.h"; then
               if test -f "$_json_path_tmp/lib/libjson.a" && test -r "$_json_path_tmp/lib/libjson.a"; then
                  _json_cppflags="-I$_json_path_tmp/include/json"
                  _json_ldflags="-L$_json_path_tmp/lib"
                  _json_libs="$_json_path_tmp/lib/libjson.a"
                  found_json="yes"
                  break;
               fi
           fi
        done

        if test "x$found_json" = "xyes"; then
            JSON_CPPFLAGS="$_json_cppflags"
            JSON_LDFLAGS="$_json_ldflags"
            JSON_LIBS="$_json_libs"

            AC_SUBST(JSON_CPPFLAGS)
            AC_SUBST(JSON_LDFLAGS)
            AC_SUBST(JSON_LIBS)
        fi
    fi
])

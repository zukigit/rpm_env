# LIBTAR_CHECK_CONFIG ([DEFAULT-ACTION])
# ----------------------------------------------------------
#    Job Arranger production committee        Oct-17-2014
#
# Checks for libtar.  DEFAULT-ACTION is the string yes or no to
# specify whether to default to --with-libtar or --without-libtar.
# If not supplied, DEFAULT-ACTION is no.
#
# This macro is distributed in the hope that it will be useful,
# but WITHOUT ANY WARRANTY; without even the implied warranty of
# MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.

AC_DEFUN([LIBTAR_CHECK_CONFIG],
[
    AC_ARG_WITH([libtar],
        AC_HELP_STRING(
            [--with-libtar@<:@=ARG@:>@],
            [use libtar library @<:@default=yes@:>@, optionally specify the prefix for libtar library]
        ),
        [
            WANT_LIBTAR="yes"
            _libtar_path="$withval"
        ],
        [WANT_LIBTAR="yes"]
    )

    found_libtar="no"
    LIBTAR_CFLAGS=""
    LIBTAR_CPPFLAGS=""
    LIBTAR_LDFLAGS=""
    LIBTAR_LIBS=""
    if test "x$WANT_LIBTAR" = "xyes"; then
	AC_MSG_CHECKING(for LIBTAR support)
        for _libtar_path_tmp in $_libtar_path /usr /usr/local /opt ; do
           if test -f "$_libtar_path_tmp/include/libtar.h" && test -r "$_libtar_path_tmp/include/libtar.h"; then
               if test -f "$_libtar_path_tmp/lib/libtar.a" && test -r "$_libtar_path_tmp/lib/libtar.a"; then
                  _libtar_cppflags="-I$_libtar_path_tmp/include"
                  _libtar_ldflags="-L$_libtar_path_tmp/lib"
                  _libtar_libs="$_libtar_path_tmp/lib/libtar.a"
                  found_libtar="yes"
                  break;
               fi
           fi
        done

        if test "x$found_libtar" = "xyes"; then
            LIBTAR_CPPFLAGS="$_libtar_cppflags"
            LIBTAR_LDFLAGS="$_libtar_ldflags"
            LIBTAR_LIBS="$_libtar_libs"

            AC_SUBST(LIBTAR_CPPFLAGS)
            AC_SUBST(LIBTAR_LDFLAGS)
            AC_SUBST(LIBTAR_LIBS)
        fi
    fi
])

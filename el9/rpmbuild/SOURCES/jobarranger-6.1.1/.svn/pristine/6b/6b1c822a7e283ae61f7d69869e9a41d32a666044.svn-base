Name:           jobarranger-manager
Version:        6.1.1
Release:        1%{?dist}
Summary:        Job Arranger Manager.
Vendor:         Daiwa Institute of Research Ltd

Group:		Application/Internet
License:        Apache 2.0
URL:            https://www.jobarranger.info/redmine/
Source: 	%{name}-%{version}.tar.gz
BuildRoot:      %{_tmppath}/%{name}-%{version}-root

Requires: php-common >= 7.2
Requires: php-cli >= 7.2
Requires: php-xml >= 7.2
Requires: php-pdo >= 7.2
Requires: php-fpm >= 7.2
Requires: php-mbstring >= 7.2
Requires: php-json >= 7.2
Requires: httpd

%description
Job Arranger Manager is a client terminal for editing job nets and checking job operation status.

%prep
%setup0 -q -n %{name}-%{version}

%install
mkdir -p $RPM_BUILD_ROOT%{_datadir}
mkdir -p $RPM_BUILD_ROOT%{_sysconfdir}/jobarranger/web
mkdir -p $RPM_BUILD_ROOT%{_sysconfdir}/httpd/conf.d
cp -a %{_builddir}/%{name}-%{version}/jobarranger %{buildroot}/%{_datadir}
install -Dm 0644 -p jam-cleanup.service $RPM_BUILD_ROOT%{_unitdir}/jam-cleanup.service
install -Dm 0644 -p jobarranger-ui.conf $RPM_BUILD_ROOT%{_sysconfdir}/httpd/conf.d/jobarranger-ui.conf
install -Dm 0644 -p jobarranger-api.conf $RPM_BUILD_ROOT%{_sysconfdir}/httpd/conf.d/jobarranger-api.conf


%clean
rm -rf ${buildroot}

%files
%attr(0755,apache,apache) %{_datadir}/jobarranger
%attr(0755,apache,apache) %{_sysconfdir}/jobarranger/web
%config(noreplace) %{_sysconfdir}/httpd/conf.d/jobarranger-ui.conf
%config(noreplace) %{_sysconfdir}/httpd/conf.d/jobarranger-api.conf
%{_unitdir}/jam-cleanup.service

%post
%systemd_post jam-cleanup.service
if [ -f %{_sysconfdir}/jobarranger/web/jam.config.php ]; then
 echo "backup jam.config.php file"
 mv %{_sysconfdir}/jobarranger/web/jam.config.php \
 %{_sysconfdir}/jobarranger/web/jam.config.php.rpmsave
fi

%preun
%systemd_preun jam-cleanup.service
if [ "$1" = "0" ]; then
 if [ -f $RPM_BUILD_ROOT%{_sysconfdir}/jobarranger/web/jam.config.php ]; then
  mv %{_sysconfdir}/jobarranger/web/jam.config.php \
  %{_sysconfdir}/jobarranger/web/jam.config.php.rpmsave
  echo 'warning: /etc/jobarranger/web/jam.config.php saved as /etc/jobarranger/web/jam.config.php.rpmsave' >/dev/stdout;
 fi
fi

%postun
%systemd_postun_with_restart jam-cleanup.service

%changelog
* Mon Nov 13 2023 Copyright Daiwa Institute of Research Ltd. All Rights Reserved. <https://www.jobarranger.info/jaz/jaz_release_note.html> 6.1.1
- Lastest RPM relese
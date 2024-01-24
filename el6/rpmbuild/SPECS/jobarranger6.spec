%define         version 6.0.4

Name:           jobarranger
Version:        %{version}
Release:        1.el6
Summary:        Open-source job controller solution for your IT infrastructure
Vendor:         Daiwa Institute of Research Business Innovation Ltd.

Group:          Applications/Internet
License:        GPL
URL:            https://www.jobarranger.info/redmine/
Source0:        jobarranger-%{version}.tar.gz

Buildroot:      %{_tmppath}/%{name}-%{version}-%{release}-root-%(%{__id_u} -n)

%define is_el7 %(grep -i "release 7" /etc/redhat-release > /dev/null 2>&1 && echo 1 || echo 0)

%description
Job Arranger for ZABBIX.

%package agentd
Summary:         Job Arranger agentd
Group:           Applications/Internet
Requires:        logrotate
Requires(pre):   /usr/sbin/useradd
Requires(post)  : /sbin/chkconfig
Requires(preun) : /sbin/chkconfig
Requires(postun): /sbin/service

%description agentd
The Job Arranger agentd, to be installed on systems.

%prep
%setup0 -q -n jobarranger-%{version}

chmod -R a+rX .
chmod -R a+x configure
chmod -R a+x install-gen.sh
./install-gen.sh

# fix up some lib64 issues
%{__perl} -pi.orig -e 's|_LIBDIR=/usr/lib|_LIBDIR=%{_libdir}|g' \
    configure

%build
%configure --enable-agent
make %{?_smp_mflags}

%install
rm -rf $RPM_BUILD_ROOT
make DESTDIR=$RPM_BUILD_ROOT install

# set up some required directories
mkdir -p $RPM_BUILD_ROOT%{_sysconfdir}/init.d
mkdir -p $RPM_BUILD_ROOT%{_sysconfdir}/logrotate.d
mkdir -p $RPM_BUILD_ROOT%{_datadir}
mkdir -p $RPM_BUILD_ROOT%{_localstatedir}/log/jobarranger
mkdir -p $RPM_BUILD_ROOT%{_localstatedir}/run/jobarranger
mkdir -p $RPM_BUILD_ROOT%{_localstatedir}/lib/jobarranger
mkdir -p $RPM_BUILD_ROOT%{_localstatedir}/lib/jobarranger/tmp

# init scripts
install -m 0755 -p misc/init.d/redhat/jobarg-agentd $RPM_BUILD_ROOT%{_sysconfdir}/init.d/

# log rotation
cat misc/logrotate/jobarg-logrotate.in | sed -e 's|COMPONENT|agentd|g' > \
     $RPM_BUILD_ROOT%{_sysconfdir}/logrotate.d/jobarg-agentd

%clean
rm -rf $RPM_BUILD_ROOT

%pre agentd
/usr/sbin/useradd -c "Job Arranger user" \
        -s /sbin/nologin -r -d %{_sysconfdir}/zabbix zabbix 2> /dev/null || :

%post agentd
/sbin/chkconfig --add jobarg-agentd || :

%preun agentd
if [ $1 -eq 0 ]
then
  /sbin/service jobarg-agentd stop >/dev/null 2>&1 || :
  /sbin/chkconfig --del jobarg-agentd
  rm -fr %{_localstatedir}/lib/jobarranger
fi

%files agentd
%defattr(-,root,root,-)
%doc AUTHORS ChangeLog COPYING NEWS README
%{_mandir}/man1/jobarg_exec.1.*
%{_mandir}/man1/jobarg_get.1.*
%{_mandir}/man1/jobarg_joblogput.1.*
%{_mandir}/man1/jobarg_release.1.*
%{_mandir}/man8/jobarg_agentd.8.*
%attr(0755,zabbix,zabbix) %dir %{_localstatedir}/log/jobarranger
%attr(0755,zabbix,zabbix) %dir %{_localstatedir}/run/jobarranger
%attr(0755,zabbix,zabbix) %dir %{_localstatedir}/lib/jobarranger
%attr(0755,zabbix,zabbix) %dir %{_localstatedir}/lib/jobarranger/tmp
%attr(0755,root,root) %{_sysconfdir}/init.d/jobarg-agentd
%config(noreplace) %{_sysconfdir}/logrotate.d/jobarg-agentd
%attr(0755,zabbix,zabbix) %dir %{_sysconfdir}/jobarranger
%attr(0644,zabbix,zabbix) %config(noreplace) %{_sysconfdir}/jobarranger/jobarg_agentd.conf
%attr(0755,zabbix,zabbix) %dir %{_sysconfdir}/jobarranger/extendedjob
%attr(0755,zabbix,zabbix) %{_sysconfdir}/jobarranger/extendedjob/jobarg_command
%attr(0755,zabbix,zabbix) %{_sysconfdir}/jobarranger/extendedjob/jafcheck.sh
%attr(0755,zabbix,zabbix) %{_sysconfdir}/jobarranger/extendedjob/jafwait.sh
%attr(0755,zabbix,zabbix) %{_sysconfdir}/jobarranger/extendedjob/jareboot.sh
%attr(0755,root,root) %{_bindir}/jobarg_exec
%attr(0755,root,root) %{_bindir}/jobarg_get
%attr(0755,root,root) %{_bindir}/jobarg_release
%attr(0755,root,root) %{_bindir}/jobarg_joblogput
%attr(0755,root,root) %{_sbindir}/jobarg_agentd

%changelog
* Fri Nov 12 2021 Copyright Daiwa Institute of Research Ltd. All Rights Reserved. <https://www.jobarranger.info/jaz/jaz_release_note.html> 5.1.0
- Lastest RPM relese
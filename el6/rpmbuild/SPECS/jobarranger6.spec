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

BuildRequires:   MariaDB-devel
BuildRequires:   postgresql-devel
BuildRequires:   libssh2-devel >= 1.0.0

%description
Job Arranger for ZABBIX.

%package agentd
Summary:         Job Arranger agentd
Group:           Applications/Internet
Requires:        logrotate
Requires(pre):   /usr/sbin/useradd
Requires(post)  : systemd
Requires(preun) : systemd
Requires(postun): systemd

%description agentd
The Job Arranger agentd, to be installed on systems.

%package server-mysql
Summary:         Job Arranger server compiled to use MySQL
Group:           Applications/Internet
Requires:        logrotate
Requires:        libssh2 >= 1.0.0
Requires(pre):   /usr/sbin/useradd
##Requires:        mysql
Conflicts:       jobargj-server-postgresql

%description server-mysql
Job Arranger server compiled to use MySQL

%package server-postgresql
Summary:         Job Arranger server compiled to use PostgresSQL
Group:           Applications/Internet
Requires:        logrotate
Requires:        libssh2 >= 1.0.0
Requires(pre):   /usr/sbin/useradd
##Requires:        postgresql
Conflicts:       jobarg-server-mysql

%description server-postgresql
Job Arranger server compiled to use PostgresSQL

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
%configure --enable-agent --enable-server --with-mysql
make clean all
make %{?_smp_mflags}
mv src/jobarg_server/jobarg_server src/jobarg_server/jobarg_server_mysql
mv src/jobarg_monitor/jobarg_monitor src/jobarg_monitor/jobarg_monitor_mysql
mv src/jobarg_session/jobarg_session src/jobarg_session/jobarg_session_mysql

%configure --enable-agent --enable-server --with-postgresql
make clean all
make %{?_smp_mflags}
mv src/jobarg_server/jobarg_server src/jobarg_server/jobarg_server_postgresql
mv src/jobarg_monitor/jobarg_monitor src/jobarg_monitor/jobarg_monitor_postgresql
mv src/jobarg_session/jobarg_session src/jobarg_session/jobarg_session_postgresql

touch src/jobarg_server/jobarg_server
touch src/jobarg_monitor/jobarg_monitor
touch src/jobarg_session/jobarg_session

%install
rm -rf $RPM_BUILD_ROOT

make DESTDIR=$RPM_BUILD_ROOT install

# set up some required directories
mkdir -p $RPM_BUILD_ROOT%{_sysconfdir}/logrotate.d
mkdir -p $RPM_BUILD_ROOT%{_datadir}
mkdir -p $RPM_BUILD_ROOT%{_localstatedir}/log/jobarranger
mkdir -p $RPM_BUILD_ROOT%{_localstatedir}/run/jobarranger
mkdir -p $RPM_BUILD_ROOT%{_localstatedir}/lib/jobarranger
mkdir -p $RPM_BUILD_ROOT%{_localstatedir}/lib/jobarranger/tmp
mkdir -p $RPM_BUILD_ROOT%{_prefix}/lib/systemd/system/
mkdir -p $RPM_BUILD_ROOT%{_localstatedir}/run/jobarranger
mkdir -p $RPM_BUILD_ROOT%{_sysconfdir}/tmpfiles.d

# init scripts
install -m 0644 -p misc/service/jobarg-agentd.service $RPM_BUILD_ROOT%{_prefix}/lib/systemd/system/
install -m 0644 -p misc/service/jobarg-server.service $RPM_BUILD_ROOT%{_prefix}/lib/systemd/system/
install -m 0644 -p misc/service/jobarg-monitor.service $RPM_BUILD_ROOT%{_prefix}/lib/systemd/system/
install -m 0644 -p misc/service/jobarranger.conf $RPM_BUILD_ROOT%{_sysconfdir}/tmpfiles.d/


# log rotation
cat misc/logrotate/jobarg-logrotate.in | sed -e 's|COMPONENT|server|g' > \
     $RPM_BUILD_ROOT%{_sysconfdir}/logrotate.d/jobarg-server
cat misc/logrotate/jobarg-logrotate.in | sed -e 's|COMPONENT|monitor|g' > \
     $RPM_BUILD_ROOT%{_sysconfdir}/logrotate.d/jobarg-monitor
cat misc/logrotate/jobarg-logrotate.in | sed -e 's|COMPONENT|agentd|g' > \
     $RPM_BUILD_ROOT%{_sysconfdir}/logrotate.d/jobarg-agentd

# install
rm $RPM_BUILD_ROOT%{_sbindir}/jobarg_server
install -m 0755 -p src/jobarg_server/jobarg_server_mysql $RPM_BUILD_ROOT%{_sbindir}/
install -m 0755 -p src/jobarg_server/jobarg_server_postgresql $RPM_BUILD_ROOT%{_sbindir}/

rm $RPM_BUILD_ROOT%{_sbindir}/jobarg_monitor
install -m 0755 -p src/jobarg_monitor/jobarg_monitor_mysql $RPM_BUILD_ROOT%{_sbindir}/
install -m 0755 -p src/jobarg_monitor/jobarg_monitor_postgresql $RPM_BUILD_ROOT%{_sbindir}/

rm $RPM_BUILD_ROOT%{_sysconfdir}/jobarranger/extendedjob/jobarg_session
install -m 0755 -p src/jobarg_session/jobarg_session_mysql $RPM_BUILD_ROOT%{_sysconfdir}/jobarranger/extendedjob/
install -m 0755 -p src/jobarg_session/jobarg_session_postgresql $RPM_BUILD_ROOT%{_sysconfdir}/jobarranger/extendedjob/

%clean
rm -rf $RPM_BUILD_ROOT

%pre agentd
/usr/sbin/useradd -c "Job Arranger user" \
        -s /sbin/nologin -r -d %{_sysconfdir}/zabbix zabbix 2> /dev/null || :
if [ $1 -eq 1 ]; then
    if [ -f %{_sharedstatedir}/jobarranger/jobarg_agentd.db -o \
         -f %{_sharedstatedir}/jobarranger/jobarg_agentd.db.backup -o \
         -f %{_sharedstatedir}/jobarranger/jobarg_agentd.db.jajournal ];then
        rm -f %{_sharedstatedir}/jobarranger/jobarg_agentd.*
    fi
fi

%pre server-mysql
/usr/sbin/useradd -c "Job Arranger user" \
        -s /sbin/nologin -r -d %{_sysconfdir}/zabbix zabbix 2> /dev/null || :

%pre server-postgresql
/usr/sbin/useradd -c "Job Arranger user" \
        -s /sbin/nologin -r -d %{_sysconfdir}/zabbix zabbix 2> /dev/null || :

%post server-mysql
if [ $1 -eq 1 ]; then
  if [ ! -f %{_sbindir}/jobarg_server ]; then
     cd %{_sbindir}
     ln -s jobarg_server_mysql jobarg_server || :
     ln -s jobarg_monitor_mysql jobarg_monitor || :
  fi
  if [ ! -f %{_sysconfdir}/jobarranger/extendedjob/jobarg_session ]; then
     cd %{_sysconfdir}/jobarranger/extendedjob
     ln -s jobarg_session_mysql jobarg_session || :
  fi
fi

%post server-postgresql
if [ $1 -eq 1 ]; then
  if [ ! -f %{_sbindir}/jobarg_server ]; then
    cd %{_sbindir}
    ln -s jobarg_server_postgresql jobarg_server || :
    ln -s jobarg_monitor_postgresql jobarg_monitor || :
  fi
  if [ ! -f %{_sysconfdir}/jobarranger/extendedjob/jobarg_session ]; then
     cd %{_sysconfdir}/jobarranger/extendedjob
     ln -s jobarg_session_postgresql jobarg_session || :
  fi
fi

%preun agentd
if [ $1 -eq 0 ]
then
  if [ -e  %{_prefix}/lib/systemd/system/jobarg-agentd.service ];then
    cd %{_prefix}/lib/systemd/system
    %{_prefix}/bin/systemctl stop jobarg-agentd.service >/dev/null 2>&1 || :
  fi
  if [ -f %{_sharedstatedir}/jobarranger/jobarg_agentd.db -o \
        -f %{_sharedstatedir}/jobarranger/jobarg_agentd.db.backup -o \
        -f %{_sharedstatedir}/jobarranger/jobarg_agentd.db.jajournal ];then
    rm -f %{_sharedstatedir}/jobarranger/jobarg_agentd.*
  fi
fi

%preun server-mysql
if [ $1 -eq 0 ]; then
  cd %{_prefix}/lib/systemd/system
  %{_prefix}/bin/systemctl stop jobarg-server.service >/dev/null 2>&1 || :
  %{_prefix}/bin/systemctl stop jobarg-monitor.service >/dev/null 2>&1 || :

  if [ -L %{_sbindir}/jobarg_server ]; then
    rm -f %{_sbindir}/jobarg_server || :
  fi
  if [ -L %{_sysconfdir}/jobarranger/extendedjob/jobarg_session ]; then
    rm -f %{_sysconfdir}/jobarranger/extendedjob/jobarg_session || :
  fi
  if [ -L %{_sbindir}/jobarg_monitor ]; then
    rm -f %{_sbindir}/jobarg_monitor || :
  fi
fi

%preun server-postgresql
if [ $1 -eq 0 ]; then
  cd %{_prefix}/lib/systemd/system
  %{_prefix}/bin/systemctl stop jobarg-server.service >/dev/null 2>&1 || :
  %{_prefix}/bin/systemctl stop jobarg-monitor.service >/dev/null 2>&1 || :

  if [ -L %{_sbindir}/jobarg_server ]; then
    rm -f %{_sbindir}/jobarg_server || :
  fi
  if [ -L %{_sysconfdir}/jobarranger/extendedjob/jobarg_session ]; then
    rm -f %{_sysconfdir}/jobarranger/extendedjob/jobarg_session || :
  fi
  if [ -L %{_sbindir}/jobarg_monitor ]; then
    rm -f %{_sbindir}/jobarg_monitor || :
  fi
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
%attr(0644,root,root) /usr/lib/systemd/system/jobarg-agentd.service
%attr(0644,root,root) %{_sysconfdir}/tmpfiles.d/jobarranger.conf


%files server-mysql
%defattr(-,root,root,-)
%doc AUTHORS ChangeLog COPYING NEWS README
%doc database
%{_mandir}/man1/jobarg_exec.1.*
%{_mandir}/man1/jobarg_get.1.*
%{_mandir}/man1/jobarg_joblogput.1.*
%{_mandir}/man1/jobarg_release.1.*
%{_mandir}/man8/jobarg_server.8.*
%attr(0755,zabbix,zabbix) %dir %{_localstatedir}/log/jobarranger
%attr(0755,zabbix,zabbix) %dir %{_localstatedir}/run/jobarranger
%attr(0755,zabbix,zabbix) %dir %{_localstatedir}/lib/jobarranger/tmp
%config(noreplace) %{_sysconfdir}/logrotate.d/jobarg-server
%config(noreplace) %{_sysconfdir}/logrotate.d/jobarg-monitor
%attr(0755,zabbix,zabbix) %dir %{_sysconfdir}/jobarranger
%attr(0644,zabbix,zabbix) %config(noreplace) %{_sysconfdir}/jobarranger/jobarg_server.conf
%attr(0644,zabbix,zabbix) %config(noreplace) %{_sysconfdir}/jobarranger/jobarg_monitor.conf
%attr(0755,zabbix,zabbix) %dir %{_sysconfdir}/jobarranger/extendedjob
%attr(0755,zabbix,zabbix) %{_sysconfdir}/jobarranger/extendedjob/jobarg_command
%attr(0755,zabbix,zabbix) %{_sysconfdir}/jobarranger/extendedjob/jobarg_session_mysql
%attr(0755,zabbix,zabbix) %{_sysconfdir}/jobarranger/alert
%attr(0755,zabbix,zabbix) %{_sysconfdir}/jobarranger/monitor
%attr(0755,zabbix,zabbix) %dir %{_sysconfdir}/jobarranger/locale
%attr(0644,zabbix,zabbix) %{_sysconfdir}/jobarranger/locale/*
%attr(0755,root,root) %{_bindir}/jobarg_exec
%attr(0755,root,root) %{_bindir}/jobarg_get
%attr(0755,root,root) %{_bindir}/jobarg_release
%attr(0755,root,root) %{_bindir}/jobarg_joblogput
%attr(0755,root,root) %{_sbindir}/jobarg_server_mysql
%attr(0755,root,root) %{_sbindir}/jobarg_monitor_mysql
%attr(0644,root,root) /usr/lib/systemd/system/jobarg-server.service
%attr(0644,root,root) /usr/lib/systemd/system/jobarg-monitor.service
%attr(0644,root,root) %{_sysconfdir}/tmpfiles.d/jobarranger.conf

%files server-postgresql
%defattr(-,root,root,-)
%doc AUTHORS ChangeLog COPYING NEWS README
%doc database
%{_mandir}/man1/jobarg_exec.1.*
%{_mandir}/man1/jobarg_get.1.*
%{_mandir}/man1/jobarg_joblogput.1.*
%{_mandir}/man1/jobarg_release.1.*
%{_mandir}/man8/jobarg_server.8.*
%attr(0755,zabbix,zabbix) %dir %{_localstatedir}/log/jobarranger
%attr(0755,zabbix,zabbix) %dir %{_localstatedir}/run/jobarranger
%attr(0755,zabbix,zabbix) %dir %{_localstatedir}/lib/jobarranger/tmp
%config(noreplace) %{_sysconfdir}/logrotate.d/jobarg-server
%config(noreplace) %{_sysconfdir}/logrotate.d/jobarg-monitor
%attr(0755,zabbix,zabbix) %dir %{_sysconfdir}/jobarranger
%attr(0644,zabbix,zabbix) %config(noreplace) %{_sysconfdir}/jobarranger/jobarg_server.conf
%attr(0644,zabbix,zabbix) %config(noreplace) %{_sysconfdir}/jobarranger/jobarg_monitor.conf
%attr(0755,zabbix,zabbix) %dir %{_sysconfdir}/jobarranger/extendedjob
%attr(0755,zabbix,zabbix) %{_sysconfdir}/jobarranger/extendedjob/jobarg_command
%attr(0755,zabbix,zabbix) %{_sysconfdir}/jobarranger/extendedjob/jobarg_session_postgresql
%attr(0755,zabbix,zabbix) %{_sysconfdir}/jobarranger/alert
%attr(0755,zabbix,zabbix) %{_sysconfdir}/jobarranger/monitor
%attr(0755,zabbix,zabbix) %dir %{_sysconfdir}/jobarranger/locale
%attr(0644,zabbix,zabbix) %{_sysconfdir}/jobarranger/locale/*
%attr(0755,root,root) %{_bindir}/jobarg_exec
%attr(0755,root,root) %{_bindir}/jobarg_get
%attr(0755,root,root) %{_bindir}/jobarg_release
%attr(0755,root,root) %{_bindir}/jobarg_joblogput
%attr(0755,root,root) %{_sbindir}/jobarg_server_postgresql
%attr(0755,root,root) %{_sbindir}/jobarg_monitor_postgresql
%attr(0644,root,root) /usr/lib/systemd/system/jobarg-server.service
%attr(0644,root,root) /usr/lib/systemd/system/jobarg-monitor.service
%attr(0644,root,root) %{_sysconfdir}/tmpfiles.d/jobarranger.conf

%changelog
* Wed Jan 24 2014 Copyright Daiwa Institute of Research Ltd. All Rights Reserved. <https://www.jobarranger.info/jaz/jaz_release_note.html> 6.0.4
- Lastest RPM relese

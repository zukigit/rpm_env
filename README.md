# Procedure

1 -> **docker-compose up -d**(if the container is up alr, you don't need to run)<br/>
2 -> **move jobarranger source file to rpmbuild/SOURCE and change name to jobarranger-6.x.x**<br/>
3 -> **tar -cvzf rpmbuild/SOURCE/jobarranger-6.x.x.tar.gz rpmbuild/SOURCE/jobarranger-6.x.x** //zipped it<br/>
4 -> **change version number, date etc.. in rpmbuild/SPECS/jobarranger8.spec**<br/>
5 -> **./rpm_build.sh**<br/>
6 -> **exported rpms files are under /rpmbuild/RPMS**<br/>

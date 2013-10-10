
# description:
#   Revert last commit of ESMS SVN repository, especially useful when envolved a 
#     lot of files
# usage:
#   Navigate to the root directory of an ESMS checkout and invoke following command:
#   $ revertLastCommit.py
# author:
#   xzhang@qilinsoft.com
# changed history:
#   2012-06-28 initial version

print 'Retriving HEAD revision info ...'
from subprocess import Popen, PIPE
p = Popen('svn log -v -r HEAD', stdout=PIPE)
# svn verbose log of last commit is stored in stdout
stdout, stderr = p.communicate()

stdoutLines = stdout.splitlines()

# get head revision
headRev = int(stdoutLines[1].split('|')[0].replace('r', ''))
print 'HEAD revision:', headRev
# get changed paths
changedPaths = []
changedPathsStart = False
print 'Changed paths:'
for line in stdoutLines:
    line = line.strip()
    if line == 'Changed paths:':
        changedPathsStart = True
        continue
    if line == '':
        break
    if changedPathsStart == True:
        changePath = line.split(' ')
        changedPaths.append(changePath)
        print changePath

baseUrl = 'http://10.101.5.51/svn/tang'
oldRev = str(headRev - 1)
print '\nReverting to revision', oldRev, '...'
for path in changedPaths:
    localFile = path[1].replace('/trunk/esms/', '')
    svnUrl = baseUrl + path[1]
    if path[0] == 'M':
        p = Popen('svn export -r ' + oldRev + ' ' + svnUrl + ' ' + localFile, stdout=PIPE)
        stdout, stderr = p.communicate()
    if path[0] == 'A':
        import os
        os.remove(localFile)
    if path[0] == 'D':
        p = Popen('svn up -r ' + oldRev + ' ' + svnUrl + ' ' + localFile, stdout=PIPE)
        stdout, stderr = p.communicate()
    print 'Reverted', path[1]

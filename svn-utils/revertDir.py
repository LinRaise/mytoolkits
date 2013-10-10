#!/usr/bin/env python

import subprocess
import tarfile
import os
import datetime
import optparse
import shutil

opt = optparse.OptionParser()

# command line options
opt.add_option('--include', '-i', default=".", dest="include",
               help="specify a directory to revert, default set to current directory")

opt.add_option('--include-unversioned', '-u', action="store_true", default=False, dest="includeUnversioned",
               help="also remove unversioned files if set, default set to False")

opt.add_option('--no-backup', '-n', action="store_true", default=False, dest="noBackup",
               help="not create backup before reverting a file, default set to False")

options, arguments = opt.parse_args()

# fetch local changes from standard output of 'svn st'
p = subprocess.Popen('svn st ' + options.include,
                     stdout=subprocess.PIPE,
                     stderr=subprocess.PIPE)
output, errors = p.communicate()

# read lines from changed file list:
revert_list = []
remove_list = []
lines = output.splitlines()
for line in lines:
    # you can customize your own rules here
    if line.startswith('M') or line.startswith('A'):
        revert_list.append(line.split()[1])
    if options.includeUnversioned and line.startswith('?'):
        remove_list.append(line.split()[1])
        
if (not revert_list) and (not remove_list):
    print "\nNo local changes to handle, exit."
    exit()

for name in revert_list:
    print "reverting", name, "..."
    if not options.noBackup:
        shutil.copy(name, name + ".changed")
    p = subprocess.Popen('svn revert ' + name,
                     stdout=subprocess.PIPE,
                     stderr=subprocess.PIPE)
    output, errors = p.communicate()

for name in remove_list:
    print "removing unversioned file", name, "..."
    if not options.noBackup:
        shutil.copy(name, name + ".changed")
    os.remove(name)

print "\nDone."

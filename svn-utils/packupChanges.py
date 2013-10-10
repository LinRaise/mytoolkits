#!/usr/bin/env python

import subprocess
import tarfile
import os
import datetime
import optparse

opt = optparse.OptionParser()
opt.add_option('--include', '-i', default="")
opt.add_option('--directory', '-d', default="")
options, arguments = opt.parse_args()

# fetch changed file list from standard output of 'svn st'
p = subprocess.Popen('svn st ' + options.include,
                     stdout=subprocess.PIPE,
                     stderr=subprocess.PIPE)
output, errors = p.communicate()

# read lines from changed file list:
#  - if started with 'M' or 'A', add it to packup list
packup_list = []
lines = output.splitlines()
for line in lines:
    # you can customize your own rules here
    if line.startswith('M') or line.startswith('A'):
        packup_list.append(line.split()[1])

# packup entries in the packup list into a compressed file
curdatetime = datetime.datetime.now().strftime("%Y%m%d%H%M%S")
tarname = options.directory + "\changes_" + curdatetime + ".tar"
tar = tarfile.open(tarname, "w")
print "\nGenerating", tarname, "...\n"
for name in packup_list:
    print "adding", name
    tar.add(name)
tar.close()
print "\nDone."

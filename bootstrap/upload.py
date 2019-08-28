import argparse
import csv
import json
import requests
import sys

parser = argparse.ArgumentParser(description='Process some integers.')
parser.add_argument('--baseurl', metavar='u', help='Amphora base url')
parser.add_argument('id', metavar='i', help='Amphora id')
parser.add_argument('file', metavar='f', help='CSV file')

parser.add_argument('username', metavar='u', help='Amphora username')
parser.add_argument('password', metavar='p', help='Amphora password')

args = parser.parse_args()

if(not args.baseurl):
    args.baseurl = "https://localhost:5001"

http_headers = {'content-type': 'application/json'}

# login and get token
loginPayload = {}
loginPayload['username'] = args.username
loginPayload['password'] = args.password

r = requests.post(f"{args.baseurl}/api/authentication/request", data=json.dumps(loginPayload), headers = http_headers, verify=False)
http_headers['Authorization'] = "Bearer " + r.text

with open(args.file, newline='', encoding="UTF8") as csvfile:
    spamreader = csv.reader(csvfile, delimiter=',', quotechar='|', dialect="excel")
    first = True
    for row in spamreader:
        if(first):
            headers = row
            first = False
            print(headers)
        else:
            payloadRow = {}
            key = 0
            for value in row:
                payloadRow[headers[key].replace(u'\ufeff', '')] = value
                key = key +1

            r = requests.post(f"{args.baseurl}/api/amphorae/{args.id}/signals", data=json.dumps(payloadRow), headers = http_headers)
            if(r.status_code > 299):
                print(r.status_code)
                print(r.text)
                exit(1)


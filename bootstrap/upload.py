import argparse
import csv
import json
import requests

parser = argparse.ArgumentParser(description='Process some integers.')
parser.add_argument('--baseurl', metavar='u', help='Amphora base url')
parser.add_argument('id', metavar='i', help='Tempora id')
parser.add_argument('file', metavar='f', help='CSV file')

args = parser.parse_args()

if(not args.baseurl):
    args.baseurl = "https://localhost:5001"

http_headers = {'content-type': 'application/json'}

with open(args.file, newline='', encoding="UTF8") as csvfile:
    spamreader = csv.reader(csvfile, delimiter=',', quotechar='|', dialect="excel")
    first = True
    payload = []
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
            payload.append(payloadRow)


    r = requests.post(f"{args.baseurl}/api/temporae/{args.id}/uploadMany", data=json.dumps(payload), headers = http_headers, verify=False)
    if(r.status_code > 299):
        print(r.status_code)
        print(r.text)
        exit(1)


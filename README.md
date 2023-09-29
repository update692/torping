# torping

Periodically poll regular or onion site via Tor.

## Help output

```
torping 1.0.0.0

ERROR(S):
  Required option 'i, socks5-ip' is missing.
  Required option 'p, socks5-port' is missing.
  Required option 'u, url' is missing.

  -i, --socks5-ip      Required. Tor proxy IP address.

  -p, --socks5-port    Required. Tor proxy Port.

  -u, --url            Required. Site URL.

  -t, --timeout        (Default: 30) Connection timeout (sec).

  -n, --interval       (Default: 15) Ping interval (sec).

  -r, --retry          (Default: 5) On error retry interval (sec).

  -a, --user-agent     (Default: Mozilla/5.0 (Windows NT 10.0; Win64; x64)
                       AppleWebKit/537.36 (KHTML, like Gecko) Chrome/117.0.0.0
                       Safari/537.36) User agent.

  --help               Display this help screen.

  --version            Display version information.
```

## Example output

```
Connection timeout=30s, Ping interval=60s, Retry interval=5s, User agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/117.0.0.0 Safari/537.36
127.0.0.1:9050 -> http://eixoaclv7qvnmu5rolbdwba65xpdiditdoyp6edsre3fitad777jr3ad.onion/
29.09.2023 18:53:43 | SUCCESS | 200 OK | Length: 33459 | Time: 738
29.09.2023 18:54:46 | ERROR   | 502 ConnectionRefused | Length: 0 | Time: 31
29.09.2023 18:54:53 | ERROR   | 502 ConnectionRefused | Length: 0 | Time: 51
29.09.2023 18:55:00 | ERROR   | 502 ConnectionRefused | Length: 0 | Time: 30
29.09.2023 18:55:35 | ERROR   | A task was canceled. (Probably timeout)
29.09.2023 18:55:42 | SUCCESS | 200 OK | Length: 33459 | Time: 506
29.09.2023 18:56:44 | SUCCESS | 200 OK | Length: 33459 | Time: 120
29.09.2023 18:57:47 | SUCCESS | 200 OK | Length: 33459 | Time: 315
Interval...
```

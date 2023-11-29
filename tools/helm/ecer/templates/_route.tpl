# route template
{{- define "route.tpl" }}
{{- $values := .values -}}
{{- $name := .name -}}
{{- $labels := .labels -}}
{{- $port := ($values.port | default 8080) -}}
{{- $protocol := ($values.protocol | default "tcp") -}}
{{- range $host := $values.routes }}
kind: Route
apiVersion: route.openshift.io/v1
metadata:
  name: {{ $name }}-{{ $host.host }}-route
  labels: {{ $labels | nindent 4 }}
  annotations:
    haproxy.router.openshift.io/hsts_header: max-age=31536000;includeSubDomains;preload
    haproxy.router.openshift.io/balance: leastconn
    haproxy.router.openshift.io/timeout: {{ $values.routeTimeout | default  "60s" }}
spec:
  host: {{ $host.host }}
  path: {{ $host.path | default "" | quote }}
  port:
    targetPort: {{ printf "%d-%s" $port $protocol }}
  tls:
    insecureEdgeTerminationPolicy: Redirect
    termination: edge
    {{- if $host.key }}
    key: | {{ $values.Files.Get $host.key | trim | nindent 6 }}
    certificate: | {{ $values.Files.Get $host.certificate | trim | nindent 6 }}
    caCertificate: | {{ $values.Files.Get $host.caCertificate | trim | nindent 6 }}
    {{- end }}
  to:
    kind: Service
    name: {{ $name }}-svc
    weight: 100
---
{{- end -}}
{{- end -}}

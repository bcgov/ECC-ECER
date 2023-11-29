# network policy template
{{- define "netpol.tpl" }}
{{- $values := .values -}}
{{- $name := .name -}}
{{- $labels := .labels -}}
{{- $port := ($values.port | default 8080) -}}
{{- $protocol := ($values.protocol | default "tcp") -}}
kind: NetworkPolicy
apiVersion: networking.k8s.io/v1
metadata:
  name: {{ $name }}-netpol
  labels: {{ $labels | nindent 4 }}
spec:
  podSelector:
    matchLabels:
      name: {{ $name }}
  ingress:
    - from:
      - namespaceSelector:
          matchLabels:
            network.openshift.io/policy-group: ingress
      - podSelector:
          matchLabels:
            role: api
      ports:
        - protocol: {{ $protocol | upper }}
          port: {{ $port }}
{{- end -}}

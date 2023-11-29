{{- define "secret.tpl" }}
{{- $values := .values -}}
{{- $name := .name -}}
{{- $labels := .labels -}}
kind: Secret
apiVersion: v1
metadata:
  name: {{ $name }}-secret
  labels: {{ $labels | nindent 4 }}
data:
  {{- range $key, $value := $values.secrets }}
  {{ $key }}: {{ $value | toString | b64enc | quote }}
  {{- end -}}
{{- end -}}
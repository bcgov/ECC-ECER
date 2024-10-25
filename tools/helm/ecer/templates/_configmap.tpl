# secrets template
{{- define "configmap.tpl" }}
{{- if .Values.env }}
kind: ConfigMap
apiVersion: v1
metadata:
  name: {{ .name }}-configmap
  labels: {{ .labels | nindent 4 }}
data:
  {{- range $key, $value := .Values.env }}
  {{ $key }}: {{ $value | toString | quote }}
  {{- end }}
{{- end }}
---
{{- if .Values.files }}
kind: ConfigMap
apiVersion: v1
metadata:
  name: {{ .name }}-files-configmap
  labels: {{ .labels | nindent 4 }}
data:
  {{- range $file := .Values.files }}
  {{ base $file.dst }}: |- {{ $.Files.Get $file.src | nindent 4 }}
  {{- end -}}
{{- end -}}
{{- end -}}
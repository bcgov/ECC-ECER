# service template
{{- define "service.tpl" -}}
{{- $values := .values -}}
{{- $name := .name -}}
{{- $labels := .labels -}}
{{- $port := $values.port | default 8080 -}}
{{- $targetPort := $values.targetPort | default 8080 -}}
{{- $protocol := $values.protocol | default "tcp" -}}
kind: Service
apiVersion: v1
metadata:
  name: {{ $name }}-svc
  labels: {{ $labels | nindent 4 }}
  annotations:
    service.alpha.openshift.io/serving-cert-secret-name: {{ $name }}-ssl
spec:
  selector:
    name: {{ $name }}
  ports:
    - name: {{ (printf "%s-%s" ($port | toString) $protocol) }}
      port: {{ $port }}
      protocol: {{ $protocol | upper }}
      targetPort: {{ $targetPort }}
  type: ClusterIP
{{- end -}}

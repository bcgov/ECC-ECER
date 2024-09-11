# deployment config template
{{- define "deployment.tpl" }}
kind: Deployment
apiVersion: apps/v1
metadata:
  name: {{ .name }}
  labels: {{ .labels | nindent 4 }}
  annotations:
    image.openshift.io/triggers: '[{"from":{"kind":"ImageStreamTag","name":"{{ base .Values.image.name }}:{{ .Values.image.tag }}","namespace":"{{ .Values.image.triggerNamespace }}"},"fieldPath":"spec.template.spec.containers[?(@.name==\"{{ .name }}\")].image","pause":"false"}]'
spec:
  replicas: {{ .Values.scaling.minReplicas }}
  revisionHistoryLimit: 10
  strategy:
    type: RollingUpdate
    rollingUpdate:
      maxUnavailable: 50%
      maxSurge: 50%
  selector:
    matchLabels:
      name: {{ .name }}
  template:
    metadata:
      name: {{ .name }}
      labels:
        name: {{ .name }}
        role: {{ .Values.role }}
      {{- if or .Values.secrets (.Values.secretFiles).files }}
      annotations:
        {{- if gt (len .Values.env) 0 }}
        checksum/configmap: {{ .Values.env | toYaml | sha256sum }}
        {{- end }}
        {{- if gt (len .Values.files) 0 }}
        {{- range $src, $dst :=.Values.files }}
        checksum/{{ base $dst | replace "." "-" }}: {{ $.Files.Get $src | toYaml | sha256sum }}
        {{- end -}}
        {{- end }}
        {{- if gt (len .Values.secrets) 0 }}
        checksum/secret: {{ .Values.secrets | toYaml | sha256sum }}
        {{- end }}
        {{- if gt (len .Values.secretFiles) 0 }}
        {{- range $src, $dst :=.Values.secretFiles }}
        checksum/{{ base $dst | replace "." "-" }}: {{ $.Files.Get $src | toYaml | sha256sum }}
        {{- end -}}
        {{- end }}
      {{- end }}
    spec:
      containers:
        - name: {{ .name }}
          image: {{ .Values.image.name }}:{{ .Values.image.tag }}
          imagePullPolicy: Always
          securityContext:
            allowPrivilegeEscalation: {{ .Values.allowPrivilegeEscalation | default "false" }}
          resources: {{ .Values.resources | toYaml | nindent 12 }}

          {{- if or .Values.env or .Values.files .Values.secrets }}
          envFrom:
            {{- if or .Values.env or .Values.files .Values.secrets }}
            - configMapRef:
                name: {{.name }}-configmap
            {{- end }}
            {{- if .Values.secrets }}
            - secretRef:
                name: {{ .name }}-secret
            {{- end }}
          {{- end }}

          {{- if or $.Values.volumeMounts (or $.Values.files $.Values.secretFiles) }}
          volumeMounts:
            {{- if .Values.volumeMounts }}
            {{ .Values.volumeMounts | toYaml | nindent 12 }}
            {{- end -}}
            {{- range $src, $dst :=.Values.secretFiles }}
            - name: {{ $.name}}-secret-files
              mountPath: {{ $dst }}
              subPath: {{ base $dst }}
            {{- end }}
            {{- range $src, $dst :=.Values.files }}
            - name: {{ $.name}}-files
              mountPath: {{ $dst }}
              subPath: {{ base $dst }}
            {{- end -}}
            {{- end }}

          {{- if .Values.port }}
          ports:
            - containerPort: {{ .Values.port }}
              protocol: {{ .Values.protocol | default "TCP" | upper }}
          {{- end }}

          {{- if .Values.livenessProbe }}
          livenessProbe: {{ .Values.livenessProbe | toYaml | nindent 12 }}
          {{- end }}

          {{- if .Values.readinessProbe }}
          readinessProbe: {{ .Values.readinessProbe | toYaml | nindent 12 }}
          {{- end }}

          {{- if .Values.startupProbe }}
          startupProbe: {{ .Values.startupProbe | toYaml | nindent 12 }}
          {{- end }}

      dnsPolicy: ClusterFirst
      restartPolicy: Always
      terminationGracePeriodSeconds: 30

      {{- if (or $.Values.volumes (or ($.Values.files).files ($.Values.secretFiles).files)) }}
      volumes:
        {{- if .Values.volumes -}}
        {{ tpl (.Values.volumes | toYaml) $ | nindent 8 }}
        {{- end -}}
        {{- if .Values.files }}
        - name: {{ $.name}}-files
          configMap:
            name: {{ $.name}}-files-configmap              
        {{- end -}}
        {{- if .Values.secretFiles }}
        - name: {{ $.name}}-secret-files
          secret:
            secretName: {{ $.name}}-files-secret
        {{- end -}}
      {{- end }}
{{- end }}

# deployment config template
{{- define "dc.tpl" }}
{{- $values := .values -}}
{{- $name := .name -}}
{{- $labels := .labels -}}
{{- $port := ($values.port | default 8080) -}}
{{- $protocol := ($values.protocol | default "tcp") -}}
kind: DeploymentConfig
apiVersion: apps.openshift.io/v1
metadata:
  name: {{ $name }}
  labels: {{ $labels | nindent 4 }}
spec:
  replicas: {{ $values.replicas }}
  revisionHistoryLimit: 10
  strategy:
    type: Rolling
    rollingParams:
      maxUnavailable: 50%
      maxSurge: 50%
    resources:
      limits:
        cpu: 15m
        memory: 64Mi
      requests:
        cpu: 5m
        memory: 32Mi
  selector:
    name: {{ $name }}
  template:
    metadata:
      name: {{ $name }}
      labels:
        name: {{ $name }}
        role: {{ $values.role }}
    spec:
      containers:
        - name: {{ $name }}
          image: {{ $values.image.name}}:{{ $values.image.tag }}
          imagePullPolicy: Always
          resources: {{ $values.resources | toYaml | nindent 12 }}
          volumeMounts:
            - mountPath: /ssl
              name: ssl
              readOnly: true
          {{- if $values.env }}
          env:
          {{- range $key, $value := $values.env }}
            - name: {{ $key }}
              value: {{ $value | quote }}
          {{- end }}
          {{- end }}
          envFrom:
            - secretRef:
                name: {{ $name }}-secret
          ports:
            - containerPort: {{ $port }}
              protocol: {{ $protocol | upper }}
          livenessProbe:
            httpGet:
              path: {{ ($values.livenessProbe).path | default "/health" }}
              port: {{ ($values.livenessProbe).port | default $port }}
              scheme: HTTP
            timeoutSeconds: {{ ($values.livenessProbe).timeoutSeconds | default 10 }}
            periodSeconds: {{ ($values.livenessProbe).periodSeconds | default 15 }}
            failureThreshold: {{ ($values.livenessProbe).failureThreshold | default 5 }}
          readinessProbe:
            httpGet:
              path: {{ ($values.readinessProbe).path | default "/health" }}
              port: {{ ($values.readinessProbe).port | default $port }}
              scheme: HTTP
            timeoutSeconds: {{ ($values.readinessProbe).timeoutSeconds | default 10 }}
            periodSeconds: {{ ($values.readinessProbe).periodSeconds | default 15 }}
            failureThreshold: {{ ($values.readinessProbe).failureThreshold | default 5 }}
          startupProbe:
            httpGet:
              path: {{ ($values.startupProbe).path | default "/health" }}
              port: {{ ($values.startupProbe).port | default $port }}
              scheme: HTTP
            initialDelaySeconds: {{ ($values.startupProbe).initialDelaySeconds | default 15 }}
            timeoutSeconds: {{ ($values.startupProbe).timeoutSeconds | default 10 }}
            periodSeconds: {{ ($values.startupProbe).periodSeconds | default 15 }}
            failureThreshold: {{ ($values.startupProbe).failureThreshold | default 5 }}

      dnsPolicy: ClusterFirst
      restartPolicy: Always
      terminationGracePeriodSeconds: 30
      volumes:
        - name: ssl
          secret:
            secretName: {{ $name }}-ssl
  triggers:
    - type: ConfigChange
    - type: ImageChange
      imageChangeParams:
        automatic: true
        containerNames:
          - {{ $name }}
        from:
          kind: ImageStreamTag
          name: {{ base $values.image.name }}:{{ $values.image.tag }}
          namespace: {{ $values.image.triggerNamespace }}
{{- end }}

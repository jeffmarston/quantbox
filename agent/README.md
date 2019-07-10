Agent runs on all Eze machines, client, server, db.
run as a service, host a REST API
Call a central REST API



Service
 - GET:  /services/{machine}
 - POST: /service/{machine}/{svc}?cmd=start
 - POST: /service/{machine}/{svc}/?cmd=stop

 - SigR: channel="service" object="serviceParams"
 - SigR: channel="service" object="serviceParams-{name}"

Installs
 - POST: /install/
    - msi file location
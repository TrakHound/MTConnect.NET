# W3C schemas

The schemas in this directory are verbatim copies of W3C-published XSDs
that some MTConnect XSDs reference but do not ship alongside the
published MTConnect bundle. They are bundled here so the test harness can
resolve cross-namespace imports without going to the network.

| File | Source URL | Last refreshed |
|---|---|---|
| `xlink.xsd` | https://www.w3.org/1999/xlink.xsd | 2026-04 |

## Why this matters

Every MTConnect XSD from v1.5 onwards declares

```xml
<xs:import namespace="http://www.w3.org/1999/xlink" schemaLocation="xlink.xsd"/>
```

but the published spec bundles do not include `xlink.xsd`. Without this
file, any compliant XSD validator that respects `xs:import` will refuse
to compile the MTConnect XSD with errors of the shape

```
Type 'http://www.w3.org/1999/xlink:hrefType' is not declared, or is not a simple type.
The 'http://www.w3.org/1999/xlink:type' attribute is not declared.
```

cppagent and other independent consumers work around this by pre-loading
the XLink XSD into the validator's schema set before adding any MTConnect
XSD. This directory makes that pre-load reproducible from the test
assembly's manifest, with no external network access at test time.

## Refreshing

The W3C XLink 1.1 schema is stable and has not changed since publication
in 2010. If a refresh is needed, replace the file with the response body
of `curl -L https://www.w3.org/1999/xlink.xsd` and update the date column
above.

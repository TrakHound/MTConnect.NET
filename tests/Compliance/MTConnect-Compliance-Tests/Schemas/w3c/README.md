# W3C XSDs (third-party, embedded for offline schema resolution)

This directory holds verbatim copies of two W3C XSD documents used by the
MTConnect schemas at load time:

| File        | Source                              |
|-------------|-------------------------------------|
| `xlink.xsd` | https://www.w3.org/1999/xlink.xsd   |
| `xml.xsd`   | https://www.w3.org/2001/xml.xsd     |

MTConnect XSDs from v1.3 onwards reference `xlink:hrefType` and
`xlink:type` and declare an `<xs:import namespace='http://www.w3.org/1999/xlink'
schemaLocation='xlink.xsd'/>` directive that points at a sibling file the
TrakHound source tree has never shipped. With the compliance harness's
`XmlResolver = null` defense-in-depth setting the import cannot fetch the
XSD over HTTP, so the L1 schema-load gate has to seed a copy of the W3C
xlink schema into the `XmlSchemaSet` before adding the MTConnect XSD.
The xlink XSD itself imports `http://www.w3.org/XML/1998/namespace`
(`xml.xsd`) for its `xml:lang` attribute reference, which is why both
files live here.

The W3C document license permits unmodified redistribution
(https://www.w3.org/copyright/document-license/). The files in this
directory are byte-for-byte copies of the canonical W3C-hosted documents
and are not part of the MTConnect-licensed source tree.

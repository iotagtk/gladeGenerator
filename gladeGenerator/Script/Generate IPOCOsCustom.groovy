import com.intellij.database.model.DasTable
import com.intellij.database.model.ObjectKind
import com.intellij.database.util.Case
import com.intellij.database.util.DasUtil

typeMapping = [
        (~/(?i)^bit|bool|bolean$/)                        : "bool",
        (~/(?i)^tinyint$/)                                : "byte",
        (~/(?i)^uniqueidentifier|uuid$/)                  : "Guid",
        (~/(?i)^int|integer|number$/)                     : "int",
        (~/(?i)^bigint$/)                                 : "long",
        (~/(?i)^varbinary|image$/)                        : "byte[]",
        (~/(?i)^double|float|real$/)                      : "double",
        (~/(?i)^decimal|money|numeric|smallmoney$/)       : "decimal",
        (~/(?i)^datetimeoffset$/)                         : "DateTimeOffset",
        (~/(?i)^datetime|datetime2|timestamp|date|time$/) : "DateTime",
        (~/(?i)^array$/)                                  : "List<string>",
        (~/(?i)^char$/)                                   : "char",
]

notNullableTypes = [ "string", "byte[]" ]

FILES.chooseDirectoryAndSave("Choose directory", "Choose where to store generated files") { dir ->
    SELECTION.filter { it instanceof DasTable && it.getKind() == ObjectKind.TABLE }.each { generate(it, dir) }
}

def generate(table, dir) {
    def className = table.getName()
    def fields = calcFields(table)
    new File(dir, className + ".cs").withPrintWriter { out -> generate(out, className, fields) }
}

def generate(out, className, fields) {
    out.println "using System;"
    out.println ""
    out.println "public class $className"
    out.println "{"

    fields.each() {
        out.println "   public ${it.type} ${it.name} { get; set; } = ${it.defaultV};"
    }
    out.println "}"
}

def calcFields(table) {
    DasUtil.getColumns(table).reduce([]) { fields, col ->
        def spec = Case.LOWER.apply(col.getDataType().getSpecification())
        def typeStr = typeMapping.find { p, t -> p.matcher(spec).find() }?.value ?: "string"
        def nullable = col.isNotNull() || typeStr in notNullableTypes ? "" : ""
        def columnDefault = col.getDefault()
        fields += [[
                           name : col.getName(),
                           type : typeStr + nullable ,
                           defaultV : columnDefault
                           ]]
    }
}

def csharpName(str) {
    com.intellij.psi.codeStyle.NameUtil.splitNameIntoWords(str)
            .collect { Case.LOWER.apply(it).capitalize() }
            .join("")
}

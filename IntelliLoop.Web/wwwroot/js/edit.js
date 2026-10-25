function conceptRow(index)
{
    return `
        <li class="concept-item">
            <span class="concept-bullet"></span>
            <div>
                <textarea name="KeyConcepts[${index}]" id="KeyConcepts_${index}_" class="edit-field" rows="1" data-single-line></textarea>
            </div>

            <button type="button" class="row-action delete-concept" aria-label="Delete concept" title="Delete concept">
                <i class="bi bi-trash"></i>
            </button>
        </li>`;
}

function noteRow(index)
{
    return `
        <li class="note-item note-new">
            <div class="note-field">
                <label for="Notes_${index}__Title" class="note-label">Title:</label>
                <textarea name="Notes[${index}].Title" id="Notes_${index}__Title" class="edit-field note-title" rows="1" data-single-line></textarea>
            </div>

            <div class="note-field">
                <label for="Notes_${index}__Content" class="note-label">Content:</label>
                <textarea name="Notes[${index}].Content" id="Notes_${index}__Content" class="edit-field" rows="1"></textarea>
            </div>

            <button type="button" class="row-action delete-note" aria-label="Delete note" title="Delete note">
                <i class="bi bi-trash"></i>
            </button>
        </li>`;
}

const needsAutosize = !CSS.supports("field-sizing", "content");

function fit(textarea)
{
    if (!needsAutosize)
    {
        return;
    }

    textarea.style.overflow = "hidden";
    textarea.style.height = "auto";
    textarea.style.height = textarea.scrollHeight + "px";
}

function fitAll()
{
    document.querySelectorAll("textarea.edit-field").forEach(fit);
}

document.addEventListener("input", function (event)
{
    const field = event.target;

    if (!field.matches("textarea.edit-field"))
    {
        return;
    }

    if (field.matches("[data-single-line]") && /[\r\n]/.test(field.value))
    {
        field.value = field.value.replace(/[\r\n]+/g, " ");
    }

    fit(field);
});

document.addEventListener("keydown", function (event)
{
    if (event.key === "Enter" && event.target.matches("textarea[data-single-line]"))
    {
        event.preventDefault();
    }
});

if (needsAutosize)
{
    fitAll();

    window.addEventListener("resize", fitAll);
    document.fonts?.ready.then(fitAll);
}

document.addEventListener("click", function (event)
{

    if (event.target.closest("#add-concept"))
    {
        const list = document.getElementById("concept-list");
        const index = list.querySelectorAll(".concept-item").length;

        list.insertAdjacentHTML("beforeend", conceptRow(index));

        const row = list.lastElementChild;

        row.querySelectorAll("textarea").forEach(fit);
        row.querySelector("textarea").focus();

        return;
    }

    if (event.target.closest("#add-note"))
    {
        const list = document.querySelector(".notes-list");
        const index = list.querySelectorAll(".note-item").length;

        list.insertAdjacentHTML("beforeend", noteRow(index));

        const row = list.lastElementChild;

        row.querySelectorAll("textarea").forEach(fit);
        row.querySelector("textarea").focus();

        return;
    }

    const deleteNote = event.target.closest(".delete-note");

    if (deleteNote)
    {
        const note = deleteNote.closest(".note-item");

        note.querySelector("textarea[name$='.Title']").value = "";
        note.querySelector("textarea[name$='.Content']").value = "";
        note.classList.add("d-none");

        return;
    }

    const deleteConcept = event.target.closest(".delete-concept");

    if (deleteConcept)
    {
        deleteConcept.closest(".concept-item").remove();

        reindexConcepts();
    }
});

function reindexConcepts()
{
    document.querySelectorAll(".concept-item").forEach((item, index) =>
    {
        const input = item.querySelector("textarea");
        const validation = item.querySelector("[data-valmsg-for]");

        input.name = `KeyConcepts[${index}]`;
        input.id = `KeyConcepts_${index}_`;

        if (validation)
        {
            validation.setAttribute("data-valmsg-for", `KeyConcepts[${index}]`);
            validation.id = `KeyConcepts_${index}__validationMessage`;
        }
    });
}
import { Pipe, PipeTransform } from "@angular/core";
@Pipe({ name: "search", standalone: true })
export class SearchPipe implements PipeTransform {
  transform(
    items: any[] | null | undefined,
    term: string,
    keys: string[],
  ): any[] {
    if (!items || !term) return items || [];
    const q = term.toLowerCase();
    return items.filter((it) =>
      keys.some((k) =>
        String(it[k] ?? "")
          .toLowerCase()
          .includes(q),
      ),
    );
  }
}
